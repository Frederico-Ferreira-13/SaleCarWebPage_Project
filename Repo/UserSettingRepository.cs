using Core.Model;
using Contracts.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class UserSettingRepository : GenericRepository<UserSettings>, IUserSettingRepository
    {
        public UserSettingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserSettings?> GetByUserIdAsync(int userId)
        {
            return await _context.Set<UserSettings>()
                                 .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}