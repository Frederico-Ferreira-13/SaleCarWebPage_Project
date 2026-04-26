using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public CarService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<Car>> GetCarByIdAsync(int carId, int? currentUserId)
        {
            if (carId <= 0)
            {
                return Result<Car>.Failure(
                    Error.Validation(
                        "ID do carro inválido.",
                        new Dictionary<string, string[]> { { nameof(carId), new[] { "Deve ser maior que zero" } } }
                    )
                );
            }

            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null)
            {
                return Result<Car>.Failure(
                    Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Carro com ID {carId} não encontrado.")
                );
            }

            car.FavoriteCount = await _unitOfWork.Favorites.GetCountByCarIdAsync(carId);
            car.SetImageUrl(car.ImageUrl ?? "");

            if (currentUserId.HasValue)
            {
                car.IsFavorite = await _unitOfWork.Favorites.ExistsAsync(carId, currentUserId.Value);
            }

            return Result<Car>.Success(car);
        }

        public async Task<bool> ExistsAsync(int carId) =>
            carId > 0 && (await _unitOfWork.Cars.GetByIdAsync(carId) != null);

        public async Task<Result<IEnumerable<Car>>> GetPendingApprovalAsync()
        {
            var cars = await _unitOfWork.Cars.GetAllAsync();
            return Result<IEnumerable<Car>>.Success(cars.Where(c => !c.IsApproved && c.IsActive));
        }

        public async Task<Result<IEnumerable<Car>>> GetFavoriteCarByUserIdAsync(int userId)
        {
            var favs = await _unitOfWork.Favorites.GetFavoriteCarsByUserIdAsync(userId);
            return Result<IEnumerable<Car>>.Success(favs);
        }

        public async Task<Result<Car>> GetCarDetailsAsync(int carId)
        {
            return await GetCarByIdAsync(carId, null);
        }

        public async Task<Result<IEnumerable<Car>>> GetCarsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                return Result<IEnumerable<Car>>.Failure(
                    Error.Validation(
                        "ID do utilizador inválido.",
                        new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }
                    )
                );
            }

            var userCars = await _unitOfWork.Cars.GetCarsByUserIdAsync(userId);
            return Result<IEnumerable<Car>>.Success(userCars);
        }

        public async Task<(IEnumerable<Car> Items, int TotalCount)> SearchCarsAsync(string? searchTerm, int? brandId,
            int? modelId, string? fuelType, string? transmission, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            return await _unitOfWork.Cars.SearchCarsAsync(searchTerm, brandId, modelId, fuelType, transmission, page, pageSize);
        }

        public async Task<Result<Car>> CreateCarAsync(Car newCar)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful)
            {
                return Result<Car>.Failure(
                    Error.Unauthorized(
                        ErrorCodes.AuthUnauthorized,
                        "O utilizador deve estar autenticado para criar um carro."));
            }

            var modelExists = await _unitOfWork.CarModels.GetByIdAsync(newCar.CarModelId);
            if (modelExists == null)
            {
                return Result<Car>.Failure(
                    Error.Validation(
                        "O modelo de carro selecionado não existe."));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var carToCreate = new Car(
                    newCar.CarModelId,
                    newCar.ProviderId,
                    newCar.TypeOfFuel,
                    newCar.CarColor,
                    newCar.EngineCapacity,
                    newCar.CarTare,
                    newCar.Transmission,
                    newCar.Category,
                    newCar.CarPrice,
                    newCar.PlateNumber,
                    newCar.Year,
                    newCar.Kilometers
                );

                var user = await _unitOfWork.Users.GetByIdAsync(userIdResult.Value);
                if (user != null && user.UsersRoleId == 1)
                {
                    carToCreate.Approve();
                }

                await _unitOfWork.Cars.AddAsync(carToCreate);
                await _unitOfWork.CommitAsync();

                return Result<Car>.Success(carToCreate);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<Car>.Failure(Error.Validation(ex.Message));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Car>.Failure(
                    Error.InternalServer($"Erro ao criar carro: {ex.Message}"));
            }
        }

        public async Task<Result<Car>> UpdateCarAsync(int carId, Car updatedCar)
        {
            var existingCar = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (existingCar == null)
                return Result<Car>.Failure(Error.NotFound(ErrorCodes.NotFound, "Carro não encontrado para atualização."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Usar o método de atualização interno da Entidade Client
                existingCar.UpdateDetails(
                    updatedCar.CarModelId,
                    updatedCar.TypeOfFuel,
                    updatedCar.CarColor,
                    updatedCar.CarTare,
                    updatedCar.Transmission,
                    updatedCar.Category,
                    updatedCar.CarPrice,
                    updatedCar.PlateNumber,
                    updatedCar.Year,
                    updatedCar.Kilometers
                );

                if (!string.IsNullOrEmpty(updatedCar.ImageUrl))
                {
                    existingCar.SetImageUrl(updatedCar.ImageUrl);
                }

                await _unitOfWork.Cars.UpdateAsync(existingCar);
                await _unitOfWork.CommitAsync();

                return Result<Car>.Success(existingCar);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Car>.Failure(Error.InternalServer($"Erro ao atualizar Carro: {ex.Message}"));
            }
        }

        public async Task<Result> ApproveCarAsync(int carId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null)
            {
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Carro não encontrado."));
            }

            car.Approve();
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Cars.UpdateAsync(car);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateAvailabilityAsync(int carId, bool isAvailable)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null) return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Carro não encontrado."));

            if (!isAvailable) car.MarkAsSold();

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Cars.UpdateAsync(car);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteCarAsync(int carId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null) return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Carro não encontrado."));

            car.Deactivate();
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Cars.UpdateAsync(car);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<bool> AnyCarsWithModelIdAsync(int modelId)
        {
            var cars = await _unitOfWork.Cars.GetAllAsync();
            return cars.Any(c => c.CarModelId == modelId && c.IsActive);
        }

        public async Task<bool> IsCarFavoriteAsync(int carId, int userId) => await _unitOfWork.Favorites.ExistsAsync(carId, userId);

        // CORREÇÃO: Implementação ajustada para aceitar userId
        public async Task<Result<bool>> ToggleFavoriteAsync(int carId, int userId)
        {
            if (userId <= 0)
                return Result<bool>.Failure(Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Login necessário."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                bool isAlreadyFavorite = await _unitOfWork.Favorites.ExistsAsync(carId, userId);

                if (isAlreadyFavorite)
                {
                    await _unitOfWork.Favorites.DeleteAsync(carId, userId);
                }
                else
                {
                    var favorite = new Favorites(userId, carId);
                    await _unitOfWork.Favorites.AddAsync(favorite);
                }

                await _unitOfWork.CommitAsync();
                return Result<bool>.Success(!isAlreadyFavorite);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool>.Failure(Error.InternalServer(ex.Message));
            }
        }

        public async Task<int> GetFavoriteCountAsync(int carId)
        {
            return await _unitOfWork.Favorites.GetCountByCarIdAsync(carId);
        }

        public async Task<Result> UpsertCarRatingAsync(int carId, int userId, int rating) => Result.Success();
        public async Task<Result<double>> GetAverageRatingAsync(int carId) => Result<double>.Success(0.0);
    }
}