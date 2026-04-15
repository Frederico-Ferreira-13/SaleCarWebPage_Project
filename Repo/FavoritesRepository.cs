using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class FavoritesRepository : GenericRepository<Favorites>, IFavoritesRepository
    {
        public FavoritesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Favorites>()
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int carId, int userId)
        {
            return await _context.Set<Favorites>()
                                 .AnyAsync(x => x.CarId == carId && x.UserId == userId);
        }

        public async Task<int> GetCountByCarIdAsync(int carId)
        {
            return await _context.Set<Favorites>()
                                 .CountAsync(x => x.CarId == carId);
        }

        public async Task<IEnumerable<Car>> GetFavoriteCarsByUserIdAsync(int userId)
        {
            // Aqui fazemos o Join para trazer a lista de objetos Car diretamente
            return await _context.Set<Favorites>()
                                 .Where(x => x.UserId == userId)
                                 .Select(x => x.Cars) // Seleciona a propriedade de navegação
                                 .Where(car => car != null) // Garante que não venham nulos
                                 .ToListAsync()!;
        }

        public async Task DeleteAsync(int carId, int userId)
        {
            var favorite = await _context.Set<Favorites>()
                                         .FirstOrDefaultAsync(x => x.CarId == carId && x.UserId == userId);

            if (favorite != null)
            {
                _context.Set<Favorites>().Remove(favorite);                
            }
        }
    }
}