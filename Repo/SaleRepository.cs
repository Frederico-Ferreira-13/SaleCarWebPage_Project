using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Sale>> GetProposalsByCarIdAsync(int carId)
        {
            return await _context.Set<Sale>()
                    .Include(s => s.Client)
                        .ThenInclude(cl => cl.User)
                    .Where(s => s.CarId == carId)
                    .OrderByDescending(s => s.SaleDate)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetUserNegotiationsAsync(int userId, HashSet<int> userCarIds)
        {
            return await _context.Set<Sale>()
                .Include(s => s.Client)
                    .ThenInclude(cl => cl.User)
                .Where(s => s.ClientId == userId || userCarIds.Contains(s.CarId))
                .OrderByDescending(s => s.SaleDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByClientIdAsync(int clientId)
        {
            return await _context.Set<Sale>()
                 .Include(s => s.Client)
                     .ThenInclude(cl => cl.User)
                 .Where(x => x.ClientId == clientId)
                 .ToListAsync();
        }

        // Busca a venda de um carro específico
        public async Task<Sale?> GetByCarIdAsync(int carId)
        {
            return await _context.Set<Sale>()
                .Include(s => s.Client)
                    .ThenInclude(cl => cl.User)
                .FirstOrDefaultAsync(x => x.CarId == carId);
        }        
    }
}