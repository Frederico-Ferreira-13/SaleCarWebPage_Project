using Microsoft.EntityFrameworkCore;
using Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repositories;

namespace SaleCarWebPage_Project.Repo
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Car>> GetCarsForSaleAsync()
        {
            return await _context.Cars
                .Include(c => c.Model!)
                .ThenInclude(m => m.Brand)
                .Where(c => c.IsAvailable && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Car?> GetCarDetailsAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Model!)
                .ThenInclude(m => m.Brand)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        public async Task<IEnumerable<Car>> GetCarsByUserIdAsync(int userId)
        {
            // Assume que o ProviderId na tabela Car corresponde ao UserId ou está ligado a ele
            return await _context.Cars
                .Include(c => c.Model!)
                .ThenInclude(m => m.Brand)
                .Where(c => c.ProviderId == userId && c.IsActive)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Car> Items, int TotalCount)> SearchCarsAsync(
            string? searchTerm,
            int? brandId,
            int? modelId,
            string? fuelType,
            int page,
            int pageSize)
        {
            var query = _context.Cars
                 .Include(c => c.Model!)
                 .ThenInclude(m => m.Brand)
                 .Where(c => c.IsAvailable && c.IsActive)
                 .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => (c.Model != null && c.Model.ModelName.Contains(searchTerm)) ||
                                  (c.Model != null && c.Model.Brand != null && c.Model.Brand.BrandName.Contains(searchTerm)));
            }

            if (brandId.HasValue)
            {
                query = query.Where(c => c.Model != null && c.Model.BrandId == brandId.Value);
            }

            if (modelId.HasValue)
            {
                query = query.Where(c => c.ModelId == modelId.Value);
            }

            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                query = query.Where(c => c.TypeOfFuel == fuelType);
            }            

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}