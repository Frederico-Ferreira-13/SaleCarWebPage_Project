using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SaleCarWebPage_Project.Pages.Shared;
using Services;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class messagesPageModel : PageModel
    {
        private readonly IMessageBoxService _messageService;
        private readonly ICarService _carService;
        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;

        public messagesPageModel(IMessageBoxService messageService, ICarService carService, 
            ITokenService tokenService, IUsersService usersService)
        {
            _messageService = messageService;
            _carService = carService;
            _tokenService = tokenService;
            _usersService = usersService;
        }

        public List<ConversationGroup> Conversations { get; set; } = new();

        public class ConversationGroup
        {
            public int CarId { get; set; }
            public string CarName { get; set; } = string.Empty;
            public string CarImage { get; set; } = "/img/cars/default-car.jpg";
            public int OtherPartyId { get; set; }
            public string OtherPartyName { get; set; } = "Utilizador";
            public _messageBoxModel MessageBoxData { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
                return RedirectToPage("/auth");

            int currentUserId = userIdResult.Value;

            var inboxResult = await _messageService.GetUserInboxAsync(currentUserId);
            if (!inboxResult.IsSuccessful || inboxResult.Value == null)
                return Page();

            var groups = inboxResult.Value
                .GroupBy(m => new
                {
                    m.CarId,
                    OtherPartyId = m.SenderId == currentUserId ? m.ReceiverId : m.SenderId
                });

            foreach (var group in groups)
            {
                var carResult = await _carService.GetCarByIdAsync(group.Key.CarId, currentUserId);
                var car = carResult.IsSuccessful ? carResult.Value : null;

                // Buscar nome do outro utilizador
                string otherPartyName = "Utilizador";
                var userResult = await _usersService.GetUserByIdAsync(group.Key.OtherPartyId);
                if (userResult.IsSuccessful)
                    otherPartyName = userResult.Value.Name;

                // Histórico completo desta conversa
                var historyResult = await _messageService.GetChatHistoryAsync(
                    group.Key.CarId, currentUserId);

                var filteredHistory = historyResult.IsSuccessful
                    ? historyResult.Value!
                        .Where(m => m.SenderId == group.Key.OtherPartyId
                                 || m.ReceiverId == group.Key.OtherPartyId)
                        .ToList()
                    : new List<MessageBox>();

                Conversations.Add(new ConversationGroup
                {
                    CarId = group.Key.CarId,
                    CarName = car != null
                        ? $"{car.Model?.Brand?.BrandName} {car.Model?.ModelName}"
                        : $"Carro #{group.Key.CarId}",
                    CarImage = car?.ImageUrl ?? "/img/cars/default-car.jpg",
                    OtherPartyId = group.Key.OtherPartyId,
                    OtherPartyName = otherPartyName,
                    MessageBoxData = new _messageBoxModel
                    {
                        CarId = group.Key.CarId,
                        ProviderId = car?.ProviderId,
                        ChatHistory = filteredHistory,
                        ReadOnly = false
                    }
                });
            }

            Conversations = Conversations
                .OrderByDescending(c => c.MessageBoxData.ChatHistory
                    .MaxBy(m => m.SentDate)?.SentDate ?? DateTime.MinValue)
                .ToList();

            return Page();
        }
    }
}
