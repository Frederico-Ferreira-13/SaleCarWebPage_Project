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
        Task<Result<CarModel>> GetCarModelIdAsync(int modelIdd);
        Task<Result<CarModel>> GetByNameAsync(string modelName);
        Task<Result<IEnumerable<CarModel>>> GetAllCarModelAsync();
        Task<Result<CarModel>> CreateCarModelAsync(CarModel carModel);
        Task<Result<CarModel>> UpdateCarModelAsync(CarModel updateModel);
        Task<Result> DeleteCarModelAsync(int modelId);
    }
}
