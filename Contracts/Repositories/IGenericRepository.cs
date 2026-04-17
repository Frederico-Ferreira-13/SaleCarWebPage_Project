using Core.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        Task CreateAddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);

        // Como queremos apenas ler, não precisamos de um método genérico de atualização e manipolação de dados. O repositório genérico é focado em operações de leitura.
        Task<IEnumerable<TEntity>> GetAllAsync(); 
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
    }
}