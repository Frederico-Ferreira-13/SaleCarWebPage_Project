using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface ISaleService
    {
        Task<Result> AddAsync(Sale sale);

        Task<Result> MarkAsSoldAsync(int carId, int clientId, decimal finalPrice);

        Task<Result<IEnumerable<Car>>> GetSoldInventoryAsync(int providerId);

        Task<Result<IEnumerable<Sale>>> GetProposalsByCarIdAsync(int carId);

        Task<int?> GetClientIdByUserIdAsync(int userId);
    }
}
