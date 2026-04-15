using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        // Útil para ver o histórico de vendas de um cliente ou de um carro
        Task<IEnumerable<Sale>> GetByClientIdAsync(int clientId);
        Task<Sale> GetByCarIdAsync(int carId);
    }
}