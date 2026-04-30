using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using SaleCarWebPage_Project.Pages.Shared;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    public class InboxConversation
    {
        public int CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
    }

    public class viewCarModel : PageModel
    {
        private readonly ICarService _carServices;
        private readonly IMessageBoxService _messageService;
        private readonly ITokenService _tokenService;
        private readonly ISaleService _saleService;
        private readonly ILogger<viewCarModel> _logger;

        public viewCarModel(
            ICarService carServices,
            IMessageBoxService messageService,
            ITokenService tokenService,
            ISaleService saleService,
            ILogger<viewCarModel> logger)
        {
            _carServices = carServices;
            _messageService = messageService;
            _tokenService = tokenService;
            _saleService = saleService;
            _logger = logger;
        }

        public Car? Car { get; set; } = default!;
        public bool CanEdit { get; set; }

        [BindProperty]
        public string MessageText { get; set; } = string.Empty;

        [BindProperty]
        public string Subject { get; set; } = string.Empty;

        [BindProperty]
        public int? ParentMessageId { get; set; }

        public List<SelectListItem> SubjectOptions { get; set; } = new()
        {
            new SelectListItem { Value = "Pedido de Informação", Text = "Pedido de Informação" },
            new SelectListItem { Value = "Proposta de Compra", Text = "Proposta de Compra" },
            new SelectListItem { Value = "Agendar Visita/Test-Drive", Text = "Agendar Visita / Test-Drive" },
            new SelectListItem { Value = "Avaliação de Retoma", Text = "Avaliação de Retoma" }
        };

        public MessagesViewModel MessageBoxData { get; set; } = new();
        public List<InboxConversation> InboxConversations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            int? currentUserId = userIdResult.IsSuccessful ? userIdResult.Value : null;

            // --- MODO 1: INBOX GLOBAL (sem id) ---
            if (id == null || id <= 0)
            {
                if (!currentUserId.HasValue) return RedirectToPage("/auth");

                var inboxResult = await _messageService.GetUserInboxAsync(currentUserId.Value);

                if (inboxResult.IsSuccessful && inboxResult.Value != null)
                {
                    var messages = inboxResult.Value.ToList();

                    // Alimentamos o MessageBoxData para usar o design da Partial View
                    MessageBoxData = new MessagesViewModel
                    {
                        CarId = 0,
                        ChatHistory = messages
                    };

                    // Mantemos a InboxConversations para compatibilidade lógica se necessário
                    InboxConversations = messages
                        .GroupBy(m => m.CarId)
                        .Select(g => new InboxConversation
                        {
                            CarId = g.Key,
                            CarName = g.OrderByDescending(m => m.SentDate).FirstOrDefault()?.Subject ?? $"Veículo #{g.Key}",
                            LastMessageDate = g.Max(m => m.SentDate)
                        })
                        .ToList();
                }

                Car = null;
                return Page();
            }

            // --- MODO 2: DETALHES DO CARRO (id presente) ---
            var result = await _carServices.GetCarByIdAsync(id.Value, currentUserId);

            if (!result.IsSuccessful || result.Value == null)
            {
                return NotFound();
            }

            Car = result.Value;

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isAdmin = userRole == "1";
            bool isOwner = currentUserId.HasValue && Car.ProviderId == currentUserId.Value;

            CanEdit = isAdmin || isOwner;

            if (CanEdit)
            {
                var proposalsResult = await _saleService.GetProposalsByCarIdAsync(id.Value);
                if (proposalsResult.IsSuccessful)
                {
                    Car.Proposals = proposalsResult.Value!.ToList();
                }
            }

            if (currentUserId.HasValue)
            {
                var historyResult = await _messageService.GetChatHistoryAsync(id.Value, currentUserId.Value);

                MessageBoxData = new MessagesViewModel
                {
                    CarId = id.Value,
                    ProviderId = Car.ProviderId,
                    ChatHistory = historyResult.Value?.ToList() ?? new List<MessageBox>()
                };
            }
            else
            {
                MessageBoxData = new MessagesViewModel { CarId = id.Value, ChatHistory = new List<MessageBox>() };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(int id)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            if (string.IsNullOrWhiteSpace(MessageText))
            {
                return RedirectToPage(new { id });
            }

            var carResult = await _carServices.GetCarByIdAsync(id, userIdResult.Value);
            if (!carResult.IsSuccessful || carResult.Value == null) return NotFound();

            int receiverId = carResult.Value.ProviderId;

            string resolvedSubject = ParentMessageId.HasValue
                ? await GetParentSubjectAsync(ParentMessageId.Value)
                : (string.IsNullOrWhiteSpace(Subject) ? $"Interesse no veículo #{id}" : Subject);

            var result = await _messageService.SendMessageAsync(
                carId: id,
                senderId: userIdResult.Value,
                receiverId: receiverId,
                subject: resolvedSubject,
                messageText: MessageText,
                parentMessageId: ParentMessageId
            );

            if (!result.IsSuccessful)
                _logger.LogWarning("Falha ao enviar mensagem: CarId={CarId}, Erro={Msg}", id, result.Message);

            return RedirectToPage(new { id });
        }

        private async Task<string> GetParentSubjectAsync(int parentId)
        {
            var result = await _messageService.GetMessageDetailAsync(parentId, 0);
            return result.IsSuccessful ? result.Value.Subject : "Re: mensagem anterior";
        }

        public async Task<IActionResult> OnPostSubmitProposalAsync(int carId, decimal offerValue, string contact)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
                return new JsonResult(new { success = false, message = "Sessão expirada." });

            try
            {
                int realClientId = await _saleService.EnsureClientProfileExistsAsync(userIdResult.Value);

                var newProposal = new Sale(
                    carId,
                    realClientId,
                    DateTime.Now,
                    offerValue,
                    DateTime.Now,
                    $"Proposta via Web - Contacto: {contact}"
                );

                var result = await _saleService.AddAsync(newProposal);

                return new JsonResult(new
                {
                    success = result.IsSuccessful,
                    message = result.IsSuccessful ? "Proposta enviada!" : "Erro ao guardar."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Erro interno: " + ex.Message });
            }
        }

        public async Task<JsonResult> OnPostToggleFavorite([FromBody] int carId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
                return new JsonResult(new { success = false, message = "Login necessário." });

            var result = await _carServices.ToggleFavoriteAsync(carId, userIdResult.Value);

            return new JsonResult(new
            {
                success = result.IsSuccessful,
                isFavorite = result.Value,
                count = result.Message
            });
        }
    }
}