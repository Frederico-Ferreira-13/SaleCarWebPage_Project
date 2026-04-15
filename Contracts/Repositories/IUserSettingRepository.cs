using Core.Model;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUserSettingRepository : IGenericRepository<UserSettings>
    {
        // Útil para ir buscar as definições de um utilizador específico
        Task<UserSettings> GetByUserIdAsync(int userId);
    }
}