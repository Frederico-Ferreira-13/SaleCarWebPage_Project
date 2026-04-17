using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUsersRoleRepository : IGenericRepository<UsersRole>
    {
        Task<IEnumerable<UsersRole>> GetUsersByRoleIdAsync(int roleId);

        Task<UsersRole> GetByNameAsync(string roleName);
    }
}