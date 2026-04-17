using Core.Model;
using Contracts.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Provider?> GetByCompanyNameAsync(string companyName)
        {
            return await _context.Set<Provider>()
                                 .FirstOrDefaultAsync(x => x.CompanyName == companyName);
        }
    }
}