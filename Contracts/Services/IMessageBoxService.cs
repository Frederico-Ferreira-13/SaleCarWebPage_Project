using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Contracts.Services
{
    public interface IMessageBoxService
    {
        Task<Result<MessageBox>> GetMessageDetailAsync(int messageId, int currentUserId);
        Task<Result<IEnumerable<MessageBox>>> GetUserInboxAsync(int userId);
        Task<Result<IEnumerable<MessageBox>>> GetChatHistoryAsync(int carId, int buyerId, int sellerId);

        Task<Result<MessageBox>> SendInquiryAsync(int carId, int senderId, string subject, string messageText);
        Task<Result> MarkAsReadAsync(int messageId, int currentUserId);
        Task<Result> ArchiveMessageAsync(int messageId, int currentUserId);

        Task<Result<int>> GetUnreadCountAsync(int userId);

        Task<Result<IEnumerable<MessageBox>>> GetChatHistoryAsync(int carId, int currentUserId);

        Task<Result<MessageBox>> SendMessageAsync(int carId, int senderId, int receiverId, string subject,
            string messageText, int? parentMessageId = null);

        Task<Result> EditMessageAsync(int messageId, int currentUserId, string newText);
    }
}
