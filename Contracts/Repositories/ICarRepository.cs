using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        // Métodos específicos que criámos para o site de vendas
        Task<List<Car>> GetCarsForSaleAsync();
        Task<Car?> GetCarDetailsAsync(int id);

        Task<IEnumerable<Car>> GetCarsByUserIdAsync(int userId);
        Task<(IEnumerable<Car> Items, int TotalCount)> SearchCarsAsync(string? searchTerm, int? brandId, int? modelId, string? fuelType, string transmission, int page, int pageSize);
    }
}