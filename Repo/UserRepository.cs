using Core.Model;
using Contracts.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
   
    public class UserRepository : GenericRepository<Users>, IUsersRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await _context.Set<Users>()
                                 .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Users?> GetByUserNameAsync(string userName)
        {
            return await _context.Set<Users>()
                                 .FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}