using Contracts.Repositories;
using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace SaleCarWebPage_Project.Repo
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {       

        public BrandRepository(ApplicationDbContext context) : base(context)        {
            
        }

        public async Task<Brand?> GetByNameAsync(string brandName)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.BrandName.ToLower() == brandName.ToLower());
        }

        public async Task<Brand?> ReadByIdAndBrandAsync(int id, IEnumerable<Brand> allowedBrands)
        {
            // Esta implementação filtra o ID dentro da lista de marcas já carregada em memória
            return await Task.FromResult(allowedBrands.FirstOrDefault(b => b.BrandId == id));
        }
    }
}