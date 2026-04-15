using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface ICarModelRepository : IGenericRepository<CarModel>
    {
        // Se quiser listar modelos por marca:
        Task<IEnumerable<CarModel>> GetModelsByBrandIdAsync(int brandId);
    }
}