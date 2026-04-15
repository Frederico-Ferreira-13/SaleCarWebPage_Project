using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class MessageBoxRepository : GenericRepository<MessageBox>, IMessageBoxRepository
    {
        public MessageBoxRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MessageBox>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<MessageBox>()
                                 .Where(x => (x.SenderId == userId || x.ReceiverId == userId) && !x.IsDeleted)
                                 .OrderByDescending(x => x.SentDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<MessageBox>> GetChatHistoryAsync(int carId, int user1Id, int user2Id)
        {
            return await _context.Set<MessageBox>()
                                 .Where(x => x.CarId == carId &&
                                            ((x.SenderId == user1Id && x.ReceiverId == user2Id) ||
                                             (x.SenderId == user2Id && x.ReceiverId == user1Id)) &&
                                            !x.IsDeleted)
                                 .OrderBy(x => x.SentDate)
                                 .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Set<MessageBox>()
                                 .CountAsync(x => x.ReceiverId == userId && !x.IsRead && !x.IsDeleted);
        }
    }
}