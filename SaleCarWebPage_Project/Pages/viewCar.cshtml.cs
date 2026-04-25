using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SaleCarWebPage_Project.Pages.Shared;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    public class viewCarModel : PageModel
    {
        private readonly ICarService _carServices;
        private readonly IMessageBoxService _messageService;
        private readonly ITokenService _tokenService;

        public viewCarModel(ICarService carServices, IMessageBoxService messageService, ITokenService tokenService)
        {
            _carServices = carServices;
            _messageService = messageService;
            _tokenService = tokenService;
        }

        public Car Car { get; set; } = default!;

        [BindProperty]
        public string MessageText { get; set; } = string.Empty;

        [BindProperty]
        public string Subject { get; set; } = string.Empty;

        public List<SelectListItem> SubjectOptions { get; set; } = new()
        {
            new SelectListItem { Value = "Pedido de Informação", Text = "Pedido de Informação" },
            new SelectListItem { Value = "Proposta de Compra", Text = "Proposta de Compra" },
            new SelectListItem { Value = "Agendar Visita/Test-Drive", Text = "Agendar Visita / Test-Drive" },
            new SelectListItem { Value = "Avaliação de Retoma", Text = "Avaliação de Retoma" }
        };

        public _messageBoxModel MessageBoxData { get; set; }

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

            MessageBoxData = new _messageBoxModel
            {
                CarId = id,
                ProviderId = Car.ProviderId,
                ChatHistory = currentUserId.HasValue
                    ? (await _messageService.GetChatHistoryAsync(id, currentUserId.Value, Car.ProviderId)).Value?.ToList() ?? new List<MessageBox>()
                    : new List<MessageBox>()

            };

            if (currentUserId.HasValue)
            {
                var historyResult = await _messageService.GetChatHistoryAsync(id, currentUserId.Value, Car.ProviderId);

                MessageBoxData = new _messageBoxModel
                {
                    CarId = id,
                    ProviderId = Car.ProviderId,
                    ChatHistory = historyResult.Value?.ToList() ?? new List<MessageBox>()
                };

                Console.WriteLine($"\n--- DEBUG GET CHAT ---");
                Console.WriteLine($"Utilizador Atual: {currentUserId.Value}");
                Console.WriteLine($"Mensagens encontradas: {MessageBoxData.ChatHistory.Count}");

                foreach (var m in MessageBoxData.ChatHistory)
                {
                    Console.WriteLine($"Msg: {m.MessageText} | De: {m.SenderId} | Em: {m.SentDate}");
                }
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
            var result = await _messageService.SendInquiryAsync(
                id,
                userIdResult.Value,
                $"Interesse no veículo #{id}",
                MessageText
            );

            Console.WriteLine($"Resultado do Envio: {result.IsSuccessful}");
            if (!result.IsSuccessful) Console.WriteLine($"Erro do Serviço: {result.Message}");

            return RedirectToPage(new { id = id });
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
