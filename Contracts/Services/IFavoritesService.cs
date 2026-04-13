using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IFavoritesService
    {
        Task<Result<int>> GetCurrentUserIdAsync();
        Task<Result> AddFavoriteAsync(Favorites favorite);
        Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId);
        Task<Result> RemoveFavoriteAsync(int favoriteId);
        Task<Result<bool>> ToggleFavoriteAsync(int recipeId);
    }
}
