using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IClientService
    {
        Task<Result<Client>> GetByIdAsync(int clientId);
        Task<Result<IEnumerable<Client>>> GetClientAsync();
        Task<Result<Client>> CreateClientAsync(Client createClient);
        Task<Result<Client>> UpdateClientAsync(Client updatedClient);
        Task<Result> DeleteAsync(int clientId);
    }
}
