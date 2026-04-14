using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface ICarModelService
    {
        Task<Result<CarModel>> GetBrandIdAsync(int modelIdd);
        Task<Result<CarModel>> GetByNameAsync(string modelName);
        Task<Result<IEnumerable<CarModel>>> GetAllBrandsAsync();
        Task<Result<CarModel>> CreateBrandAsync(CarModel carModel);
        Task<Result<CarModel>> UpdateBrandAsync(CarModel updateModel);
        Task<Result> DeleteBrandAsync(int modelId);
    }
}
