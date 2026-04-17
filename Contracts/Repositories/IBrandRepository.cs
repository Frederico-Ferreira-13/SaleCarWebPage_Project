using Core.Model;

namespace Contracts.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        Task<Brand> ReadByIdAndBrandAsync(int id, IEnumerable<Brand> allowedBrands);
        Task<Brand> GetByNameAsync(string brandName);
    }
}