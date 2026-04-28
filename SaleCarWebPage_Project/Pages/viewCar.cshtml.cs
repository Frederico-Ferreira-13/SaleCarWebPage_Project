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
    public class viewCarModel : PageModel
    {
        private readonly ICarService _carServices;
        private readonly IMessageBoxService _messageService;
        private readonly ITokenService _tokenService;
        private readonly ISaleService _saleService;

        public viewCarModel(ICarService carServices, IMessageBoxService messageService, ITokenService tokenService, 
            ISaleService saleService)
        {
            _carServices = carServices;
            _messageService = messageService;
            _tokenService = tokenService;
            _saleService = saleService;
        }

        public Car Car { get; set; } = default!;

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

        public _messageBoxModel MessageBoxData { get; set; }

        private readonly ILogger<viewCarModel> _logger;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if(id <= 0)
            {
                return RedirectToPage("/Index");
            }

            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            int? currentUserId = userIdResult.IsSuccessful ? userIdResult.Value : null;

            var result = await _carServices.GetCarByIdAsync(id, currentUserId);

            if (!result.IsSuccessful || result.Value == null)
            {
                return NotFound();
            }

            Car = result.Value;

            CanEdit = User.IsInRole("1") || (currentUserId.HasValue && Car.ProviderId == currentUserId.Value);

            if (CanEdit)
            {
                var proposalsResult = await _saleService.GetProposalsByCarIdAsync(id);
                if (proposalsResult.IsSuccessful)
                {
                    Car.Proposals = proposalsResult.Value!.ToList();
                }
            }

            if (currentUserId.HasValue)
            {                
                var historyResult = await _messageService.GetChatHistoryAsync(id, currentUserId.Value);

                MessageBoxData = new _messageBoxModel
                {
                    CarId = id,
                    ProviderId = Car.ProviderId,
                    ChatHistory = historyResult.Value?.ToList() ?? new List<MessageBox>()
                };
            }
            else
            {
                MessageBoxData = new _messageBoxModel { CarId = id, ChatHistory = new List<MessageBox>() };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(int id)
        {
            Console.WriteLine($"\n--- DEBUG SEND MESSAGE ---");
            Console.WriteLine($"CarId: {id}");
            Console.WriteLine($"Texto Recebido: '{MessageText}'");

            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
            {
                Console.WriteLine("Erro: Utilizador não autenticado.");
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(MessageText))
            {
                Console.WriteLine("Aviso: MessageText está VAZIO. O binding pode ter falhado.");
                return RedirectToPage(new { id });
            }

            // Carregamos o carro para saber quem é o fornecedor (ReceiverId)
            var carResult = await _carServices.GetCarByIdAsync(id, userIdResult.Value);
            if (!carResult.IsSuccessful || carResult.Value == null)
                return NotFound();

            int receiverId = carResult.Value.ProviderId;

            string resolvedSubject = ParentMessageId.HasValue
        ? await GetParentSubjectAsync(ParentMessageId.Value)
        : (string.IsNullOrWhiteSpace(Subject)
            ? $"Interesse no veículo #{id}"
            : Subject);

            // 5. Enviar — passando ParentMessageId para criar a thread
            var result = await _messageService.SendMessageAsync(
                carId: id,
                senderId: userIdResult.Value,
                receiverId: receiverId,
                subject: resolvedSubject,
                messageText: MessageText,
                parentMessageId: ParentMessageId   // null = nova conversa, int = resposta
            );

            if (!result.IsSuccessful)
                _logger.LogWarning("Falha ao enviar mensagem: CarId={CarId}, Erro={Msg}",
                    id, result.Message);

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
                return new JsonResult(new { success = false, message = "Sessão expirada. Faça login novamente." });

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

        // Handler AJAX para Favoritos (Similar ao RateOnly das receitas)
        public async Task<JsonResult> OnPostToggleFavorite([FromBody] int carId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
                return new JsonResult(new { success = false, message = "Login necessário." });

            // Assume que tens este método no teu ICarsService
            var result = await _carServices.ToggleFavoriteAsync(carId, userIdResult.Value);

            return new JsonResult(new
            {
                success = result.IsSuccessful,
                isFavorite = result.Value, // O service deve retornar true/false
                count = result.Message // O service deve retornar o novo total
            });
        }

        private async Task LoadPageData(int id)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            int? currentUserId = userIdResult.IsSuccessful ? userIdResult.Value : null;

            // O GetCarByIdAsync deve fazer os Includes necessários (Brand, Model, Provider, Address)
            var result = await _carServices.GetCarByIdAsync(id, currentUserId);

            if (result.IsSuccessful)
            {
                Car = result.Value;
            }
        }
    }
}
