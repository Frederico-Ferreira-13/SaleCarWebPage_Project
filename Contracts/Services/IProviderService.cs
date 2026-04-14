using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IProviderService
    {
        Task<Result<Provider>> GetByIdAsync(int providerId);
        Task<Result<IEnumerable<Provider>>> GetProvidersAsync();
        Task<Result<Provider>> CreateProviderAsync(Provider createProvider);
        Task<Result<Provider>> UpdateProviderAsync(Provider updatedProvider);
        Task<Result> DeleteAsync(int providerId);
    }
}
