using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUsersRoleRepository : IGenericRepository<UsersRole>
    {
        // Útil para saber quais os utilizadores que têm um determinado cargo
        Task<IEnumerable<UsersRole>> GetUsersByRoleIdAsync(int roleId);
    }
}