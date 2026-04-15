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
    }
}