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

        Task<int> EnsureClientProfileExistsAsync(int userId);

        Task<Result<IEnumerable<Sale>>> GetAllAsync();

        Task<Result<IEnumerable<Sale>>> GetProposalsByUserIdAsync(int userId);

        Task<Result<IEnumerable<Sale>>> GetUserNegotiationsAsync(int userId, HashSet<int> myCarIds);

        Task<Result> AcceptProposalAsync(int saleId, int userId);

        Task<Result> DeclineProposalAsync(int saleId, int userId);

        Task<Result> CreateCounterOfferAsync(int saleId, int userId, decimal counterValue);
    }
}
