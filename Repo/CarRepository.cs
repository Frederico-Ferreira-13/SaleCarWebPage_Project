using Microsoft.EntityFrameworkCore;
using SaleCarWebPage_Project.Contracts; // Importante referenciar
using Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleCarWebPage_Project.Repo
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Car>> GetCarsForSaleAsync()
        {
            return await _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Brand)
                .Where(c => c.IsAvailable && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Car> GetCarDetailsAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Brand)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.CarId == id);
        }
    }
}