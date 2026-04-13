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
        Task<Result<Car>> GetCarByIdAsync(int carId);
        Task<Result<IEnumerable<Car>>> GetCarsByUserIdAsync(int userId);
        Task<Result<Car>> GetCarDetailsAsync(int carId);

        Task<(IEnumerable<Car> Items, int TotalCount)> SearchCarsAsync(string? searchTerm, int? brandId,
            int? modelId, string? fuelType, int page, int pageSize);

        Task<bool> ExistsAsync(int carId);

        Task<bool> AnyCarsWithModelIdAsync(int modelId);

        Task<Result<IEnumerable<Car>>> GetPendingApprovalAsync();
        Task<Result> ApproveCarAsync(int carId);
        Task<Result> UpdateAvailabilityAsync(int carId, bool isAvailable);

        Task<Result<Car>> CreateCarAsync(Car car);
        Task<Result> UpdateCarAsync(Car car);
        Task<Result> DeleteCarAsync(int carId);

        Task<Result> UpsertCarRatingAsync(int carId, int userId, int rating);
        Task<Result<double>> GetAverageRatingAsync(int carId);
    }
}
