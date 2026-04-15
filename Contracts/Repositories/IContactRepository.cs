using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        // Útil para ver os contactos de um cliente específico
        Task<IEnumerable<Contact>> GetByClientIdAsync(int clientId);
    }
}