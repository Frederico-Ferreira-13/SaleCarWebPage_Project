using Microsoft.EntityFrameworkCore;
using Core.Model;
using System.Threading.Tasks;

namespace SaleCarWebPage_Project.Repo
{
    public class UserRepository : GenericRepository<Users>
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Users> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .Include(u => u.Contact)
                .FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);
        }
    }
}