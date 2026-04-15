using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IFavoritesRepository : IGenericRepository<Favorites>
    {
        // Útil para saber quais os carros favoritos de um utilizador específico
        Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int carId, int userId);
        Task<int> GetCountByCarIdAsync(int carId);
        Task<IEnumerable<Car>> GetFavoriteCarsByUserIdAsync(int userId);
        Task DeleteAsync(int carId, int userId);
    }
}