using Contracts.Repositories;
using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages
{
    public class myFavoritesCarModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ICarService _carService;

        public myFavoritesCarModel(IUnitOfWork unitOfWork, ICarService carService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _carService = carService;
        }

        public class FavoriteRequest { public int CarId { get; set; } }

        public IEnumerable<Car> MyFavoriteCar { get; set; } = Enumerable.Empty<Car>();

        public async Task OnGetAsync()
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful || userIdResult.Value == 0)
            {
                MyFavoriteCar = Enumerable.Empty<Car>();
                return;
            }

            var userId = userIdResult.Value;

            var result = await _carService.GetFavoriteCarByUserIdAsync(userIdResult.Value);

            if (result.IsSuccessful)
            {
                MyFavoriteCar = result.Value;
                foreach (var car in MyFavoriteCar)
                {
                    car.IsFavorite = true;
                }
            }
        }

        public async Task<IActionResult> OnPostToggleFavoriteAsync([FromBody] FavoriteRequest request)
        {
            int carId = request?.CarId ?? 0;
            if (carId <= 0)
            {
                return BadRequest("ID inválido");
            }

            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }            

            try
            {
                var result = await _carService.ToggleFavoriteAsync(carId);

                if (!result.IsSuccessful) return BadRequest(result.Error.Message);

                var updatedCount = await _carService.GetFavoriteCountAsync(carId);

                return new JsonResult(new
                {
                    isFavorite = result.Value,
                    newCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
