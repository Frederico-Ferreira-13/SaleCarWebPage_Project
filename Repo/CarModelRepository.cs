using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SaleCarWebPage_Project.Repo
{
    public class CarModelRepository : GenericRepository<CarModel>, ICarModelRepository
    {
        public CarModelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CarModel>> GetModelsByBrandIdAsync(int brandId)
        {
            // Procura na tabela de Modelos onde o BrandId coincide
            return await _context.Set<CarModel>()
                                 .Where(x => x.BrandId == brandId)
                                 .ToListAsync();
        }

        public async Task<CarModel> GetByNameAsync(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName)) return null;

            return await _context.Set<CarModel>()
                                 .FirstOrDefaultAsync(x => x.ModelName.ToLower() == modelName.ToLower());
        }
    }
}