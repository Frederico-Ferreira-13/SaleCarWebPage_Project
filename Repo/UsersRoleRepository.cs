using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class UsersRoleRepository : GenericRepository<UsersRole>, IUsersRoleRepository
    {
        public UsersRoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UsersRole>> GetUsersByRoleIdAsync(int roleId)
        {
            return await _context.Set<UsersRole>()
                                 .Where(x => x.UsersRoleId == roleId)
                                 .ToListAsync();
        }
    }
}