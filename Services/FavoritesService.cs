using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _usersService;

        public FavoritesService(IUnitOfWork unitOfWork, IUsersService usersService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _usersService = usersService ?? throw new ArgumentException(nameof(usersService));
        }

        public async Task<Result<int>> GetCurrentUserIdAsync()
        {
            var userIdResult = await _usersService.GetCurrentUserIdAsync();
            if (!userIdResult.IsSuccessful || userIdResult.Value <= 0)
            {
                return Result<int>.Failure(
                    Error.Unauthorized(
                        userIdResult.ErrorCode ?? ErrorCodes.AuthUnauthorized,
                        userIdResult.Message ?? "Utilizador não autenticado."));
            }

            return Result<int>.Success(userIdResult.Value);
        }

        public async Task<Result> AddFavoriteAsync(Favorites favorite)
        {
            var currentUserIdResult = await GetCurrentUserIdAsync();
            if (!currentUserIdResult.IsSuccessful)
            {
                return Result.Failure(currentUserIdResult.Error);
            }

            int currentUserId = currentUserIdResult.Value;

            if (favorite.UserId != currentUserId)
            {
                return Result.Failure(
                    Error.Forbidden(
                        ErrorCodes.AuthForbidden,
                        "Não pode adicionar favorito para outro utilizador."));
            }

            var car = await _unitOfWork.Cars.GetByIdAsync(favorite.CarId);
            if (car == null || !car.IsActive)
            {
                return Result.Failure(
                    Error.NotFound(
                        ErrorCodes.NotFound,
                        $"Receita com ID {favorite.CarId} não encontrada ou inativa."));
            }

            var existing = await _unitOfWork.Favorites.ExistsAsync(currentUserId, favorite.CarId);
            if (existing)
            {
                return Result.Success("Receita já está nos favoritos");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var favoriteToAdd = new Favorites(currentUserId, favorite.CarId);
                await _unitOfWork.Favorites.CreateAddAsync(favoriteToAdd);
                await _unitOfWork.CommitAsync();

                return Result.Success("Favorito adicionado com sucesso.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer($"Erro ao adicionar favorito: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId)
        {
            var currentUserIdResult = await GetCurrentUserIdAsync();
            if (!currentUserIdResult.IsSuccessful)
            {
                return Result<IEnumerable<Favorites>>.Failure(currentUserIdResult.Error);
            }

            int currentUserId = currentUserIdResult.Value;

            if (userId != currentUserId)
            {
                return Result<IEnumerable<Favorites>>.Failure(
                    Error.Forbidden(
                        ErrorCodes.AuthForbidden,
                        "Não pode ver favoritos de outro utilizador."
                    )
                );
            }

            var favorites = await _unitOfWork.Favorites.GetByUserIdAsync(userId);
            return Result<IEnumerable<Favorites>>.Success(favorites);
        }

        public async Task<Result> RemoveFavoriteAsync(int carId)
        {
            var currentUserIdResult = await GetCurrentUserIdAsync();
            if (!currentUserIdResult.IsSuccessful)
            {
                return Result.Failure(currentUserIdResult.Error);
            }

            int currentUserId = currentUserIdResult.Value;

            var exists = await _unitOfWork.Favorites.ExistsAsync(carId, currentUserId);
            if (!exists)
            {
                return Result.Success("Favorito já removido.");
            }
            
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Favorites.DeleteAsync(carId, currentUserId);
                await _unitOfWork.CommitAsync();
                return Result.Success("Favorito removido com sucesso.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer($"Erro ao remover favorito: {ex.Message}"));
            }
        }

        public async Task<Result<bool>> ToggleFavoriteAsync(int carId)
        {
            var currentUserIdResult = await GetCurrentUserIdAsync();
            if (!currentUserIdResult.IsSuccessful)
            {
                return Result<bool>.Failure(currentUserIdResult.Error);
            }

            int currentUserId = currentUserIdResult.Value;

            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null || !car.IsActive)
            {
                return Result<bool>.Failure(
                    Error.NotFound(
                        ErrorCodes.NotFound,
                        $"Carro com ID {car} não encontrado ou inativo"));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                bool alreadyFavorite = await _unitOfWork.Favorites.ExistsAsync(carId, currentUserId);
                bool isNowFavorite;

                if (alreadyFavorite)
                {
                    await _unitOfWork.Favorites.DeleteAsync(carId, currentUserId);
                    isNowFavorite = false;
                }
                else
                {
                    var newFavorite = new Favorites(currentUserId, carId);
                    await _unitOfWork.Favorites.CreateAddAsync(newFavorite);
                    isNowFavorite = true;
                }

                await _unitOfWork.CommitAsync();
                return Result<bool>.Success(isNowFavorite);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool>.Failure(
                    Error.InternalServer($"Erro ao togglear favorito: {ex.Message}"));
            }
        }
    }
}
