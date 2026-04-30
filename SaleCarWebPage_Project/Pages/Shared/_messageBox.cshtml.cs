using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class MessagesModel : PageModel
    {
        private readonly IMessageBoxService _messageService;
        private readonly ICarService _carService;

        public MessagesModel(IMessageBoxService messageService, ICarService carService)
        {
            _messageService = messageService;
            _carService = carService;
        }

        // Esta lista conterá apenas a última mensagem de cada conversa única
        public List<MessageBox> InboxConversations { get; set; } = new();

        // Dicionário opcional para guardarmos os nomes dos modelos dos carros para exibição rápida
        public Dictionary<int, string> CarNames { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Obter o ID do utilizador logado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return RedirectToPage("/auth");
            }

            // 2. Procurar todas as mensagens onde o utilizador participa (Inviadas ou Recebidas)
            var result = await _messageService.GetUserInboxAsync(currentUserId);

            if (result.IsSuccessful && result.Value != null)
            {
                // 3. Lógica de Agrupamento (O "Cérebro" da Inbox)
                // Agrupamos por Carro e pela "Outra Pessoa" da conversa
                InboxConversations = result.Value
                    .OrderByDescending(m => m.SentDate) // Mais recentes primeiro
                    .GroupBy(m => new
                    {
                        m.CarId,
                        // Se eu enviei, a outra pessoa é o Receiver. Se eu recebi, é o Sender.
                        OtherPartyId = (m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                    })
                    .Select(group => group.First()) // Pegamos apenas a mensagem mais recente de cada par
                    .ToList();

                // 4. (Opcional) Carregar nomes dos carros para a UI não mostrar apenas IDs
                foreach (var msg in InboxConversations)
                {
                    if (!CarNames.ContainsKey(msg.CarId))
                    {
                        var carResult = await _carService.GetCarByIdAsync(msg.CarId, null);
                        if (carResult.IsSuccessful && carResult.Value != null)
                        {
                            var name = $"{carResult.Value.Model?.Brand?.BrandName} {carResult.Value.Model?.ModelName}";
                            CarNames.TryAdd(msg.CarId, name);
                        }
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostMarkAsReadAsync(int messageId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int currentUserId))
            {
                await _messageService.MarkAsReadAsync(messageId, currentUserId);
            }
            return RedirectToPage();
        }
    }
}