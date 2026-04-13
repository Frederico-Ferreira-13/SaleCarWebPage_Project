using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface ICarRepository
    {
        // Métodos básicos (CRUD)
        Task<Car> GetByIdAsync(int id);
        Task<List<Car>> GetAllAsync();
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);

        // Métodos específicos que criámos para o site de vendas
        Task<List<Car>> GetCarsForSaleAsync();
        Task<Car> GetCarDetailsAsync(int id);
    }
}