using Core.Model;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        
        Task<Users?> GetByEmailAsync(string email);
        Task<Users?> GetByUserNameAsync(string userName);

        Task<IEnumerable<Users>> FindAsync(Expression<Func<Users, bool>> predicate);
    }
}