using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contact>> GetByClientIdAsync(int clientId)
        {
            return await _context.Set<Contact>()
                                 .Where(x => x.ClientId == clientId)
                                 .ToListAsync();
        }
    }
}