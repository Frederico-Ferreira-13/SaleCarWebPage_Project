using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IFavoritesRepository : IGenericRepository<Favorites>
    {
        // Útil para saber quais os carros favoritos de um utilizador específico
        Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId);
    }
}