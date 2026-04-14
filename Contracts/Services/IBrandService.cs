using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IBrandService
    {
        Task<Result<Brand>> GetBrandIdAsync(int brandId);
        Task<Result<Brand>> GetByNameAsync(string brandName);
        Task<Result<IEnumerable<Brand>>> GetAllBrandsAsync();
        Task<Result<Brand>> CreateBrandAsync(Brand brand);
        Task<Result<Brand>> UpdateBrandAsync(Brand updateBrand);
        Task<Result> DeleteBrandAsync(int brandId);
    }
}
