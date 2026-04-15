using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IMessageBoxRepository : IGenericRepository<MessageBox>
    {
        // Útil para listar mensagens que ainda não foram lidas
        Task<IEnumerable<MessageBox>> GetByUserIdAsync(int userId);
        Task<IEnumerable<MessageBox>> GetChatHistoryAsync(int carId, int user1Id, int user2Id);
        Task<int> GetUnreadCountAsync(int userId);
    }
}