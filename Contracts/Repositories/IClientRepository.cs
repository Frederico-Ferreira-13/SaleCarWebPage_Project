using Core.Model;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        // Métodos que podem ser úteis:
        Task<Client?> GetByEmailAsync(string email);
        Task<Client?> GetByNifAsync(string nif);
    }
}