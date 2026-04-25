using Contracts.Repositories;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Users>> FindAsync(Expression<Func<Users, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}