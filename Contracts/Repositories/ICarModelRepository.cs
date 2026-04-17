using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface ICarModelRepository : IGenericRepository<CarModel>
    {        
        Task<IEnumerable<CarModel>> GetModelsByBrandIdAsync(int brandId);
        Task<CarModel> GetByNameAsync(string modelName);
    }
}