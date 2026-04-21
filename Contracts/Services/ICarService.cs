using Contracts.Repositories;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface ICarService
    {
        Task<Result<Car>> GetCarByIdAsync(int carId, int? currentUserId);
        Task<Result<Car>> GetCarDetailsAsync(int carId);
        Task<Result<IEnumerable<Car>>> GetCarsByUserIdAsync(int userId);

        Task<(IEnumerable<Car> Items, int TotalCount)> SearchCarsAsync(string? searchTerm, int? brandId,
            int? modelId, string? fuelType, int page, int pageSize);

        Task<Result<Car>> CreateCarAsync(Car newCar);

        Task<Result> ApproveCarAsync(int carId);

        Task<Result> UpdateAvailabilityAsync(int carId, bool isAvailable);

        Task<Result> DeleteCarAsync(int carId);
        Task<bool> ExistsAsync(int carId);

        Task<bool> AnyCarsWithModelIdAsync(int modelId);
        Task<Result<IEnumerable<Car>>> GetPendingApprovalAsync();
        Task<Result<IEnumerable<Car>>> GetFavoriteCarByUserIdAsync(int userId);

        Task<bool> IsCarFavoriteAsync(int carId, int userId);
        Task<Result> UpsertCarRatingAsync(int carId, int userId, int rating);
        Task<Result<double>> GetAverageRatingAsync(int carId);
        Task<Result<bool>> ToggleFavoriteAsync(int carId);

        Task<int> GetFavoriteCountAsync(int carId);
    }
}
