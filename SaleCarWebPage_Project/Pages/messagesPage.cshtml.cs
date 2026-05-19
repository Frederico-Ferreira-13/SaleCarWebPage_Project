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

        [BindProperty] public string MessageText { get; set; } = string.Empty;
        [BindProperty] public string Subject { get; set; } = string.Empty;
        [BindProperty] public int? ParentMessageId { get; set; }

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

                string otherPartyName = "Utilizador";
                var userResult = await _usersService.GetUserByIdAsync(group.Key.OtherPartyId);
                if (userResult.IsSuccessful)
                    otherPartyName = userResult.Value.Name;

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
                        ChatHistory = historyResult.Value?.ToList() ?? new List<MessageBox>(),
                        ReadOnly = false,
                        OtherPartyName = otherPartyName
                    }
                });
            }

            Conversations = Conversations
                .OrderByDescending(c => c.MessageBoxData.ChatHistory
                    .OrderByDescending(m => m.SentDate)
                    .FirstOrDefault()?.SentDate ?? DateTime.MinValue)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(int id)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            if (string.IsNullOrWhiteSpace(MessageText))
                return RedirectToPage();

            var carResult = await _carService.GetCarByIdAsync(id, userIdResult.Value);
            if (!carResult.IsSuccessful || carResult.Value == null)
                return RedirectToPage();

            int senderId = userIdResult.Value;
            int receiverId;

            if (ParentMessageId.HasValue)
            {
                var parentResult = await _messageService.GetMessageDetailAsync(
                    ParentMessageId.Value, senderId);

                if (!parentResult.IsSuccessful) return RedirectToPage();

                var parentMsg = parentResult.Value;
                receiverId = parentMsg.SenderId == senderId
                    ? parentMsg.ReceiverId
                    : parentMsg.SenderId;
            }
            else
            {
                receiverId = carResult.Value.ProviderId;
            }

            string resolvedSubject = ParentMessageId.HasValue
                ? await GetParentSubjectAsync(ParentMessageId.Value)
                : (string.IsNullOrWhiteSpace(Subject)
                    ? $"Interesse no veículo #{id}"
                    : Subject);

            await _messageService.SendMessageAsync(
                carId: id,
                senderId: senderId,
                receiverId: receiverId,
                subject: resolvedSubject,
                messageText: MessageText,
                parentMessageId: ParentMessageId
            );

            return RedirectToPage(); // ← fica na messagesPage
        }

        private async Task<string> GetParentSubjectAsync(int parentId)
        {
            var result = await _messageService.GetMessageDetailAsync(parentId, 0);
            return result.IsSuccessful ? result.Value.Subject : "Re: mensagem anterior";
        }

        public async Task<IActionResult> OnPostEditMessageAsync(int messageId, string newText)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            if (string.IsNullOrWhiteSpace(newText)) return RedirectToPage();

            await _messageService.EditMessageAsync(messageId, userIdResult.Value, newText);
            return RedirectToPage();
        }
    }
}
