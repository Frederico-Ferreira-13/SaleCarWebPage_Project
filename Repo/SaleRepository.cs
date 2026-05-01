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
        public SaleRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Busca todas as vendas associadas a um cliente
        public async Task<IEnumerable<Sale>> GetByClientIdAsync(int clientId)
        {
            return await _context.Set<Sale>()
                                 .Where(x => x.ClientId == clientId)
                                 .ToListAsync();
        }

        // Busca a venda de um carro específico
        public async Task<Sale?> GetByCarIdAsync(int carId)
        {
            return await _context.Set<Sale>()
                                 .FirstOrDefaultAsync(x => x.CarId == carId);
        }

        public async Task<IEnumerable<Sale>> GetProposalsByCarIdAsync(int carId)
        {
            return await _context.Set<Sale>()
                    .Include(s => s.Client)
                        .ThenInclude(c => c.User)
                    .Include(s => s.Client)
                        .ThenInclude(c => c.Contact)
                    .Where(s => s.CarId == carId)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
        }
    }
}