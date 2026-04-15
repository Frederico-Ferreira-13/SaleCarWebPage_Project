using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IMessageBoxRepository : IGenericRepository<MessageBox>
    {
        // Útil para listar mensagens que ainda não foram lidas
        Task<IEnumerable<MessageBox>> GetUnreadMessagesAsync();
    }
}