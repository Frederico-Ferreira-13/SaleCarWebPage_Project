using Core.Model;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IProviderRepository : IGenericRepository<Provider>
    {
        // Útil para procurar um fornecedor pelo nome da empresa ou NIF
        Task<Provider?> GetByCompanyNameAsync(string companyName);
    }
}