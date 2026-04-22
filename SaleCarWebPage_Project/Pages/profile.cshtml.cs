using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages
{
    public class profileModel : PageModel
    {
        private readonly ICarService _carService;
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;

        public profileModel(ICarService carService, IUsersService usersService, ITokenService tokenService)
        {
            _carService = carService;
            _usersService = usersService;
            _tokenService = tokenService;
        }

        public Users? CurrentUser { get; set; }
        public int TotalAnnounced { get; set; } = 0;
        public int TotalGarage { get; set; } = 0;
        public IEnumerable<Car>? MyCars { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return RedirectToPage("/Identity/Account/Login");

            // 1. Dados do Utilizador
            var userResult = await _usersService.GetUserByIdAsync(userIdResult.Value);
            if (!userResult.IsSuccessful || userResult.Value == null)
            {
                return RedirectToPage("/Index");
            }

            CurrentUser = userResult.Value;

            // 2. Estatísticas de Carros
            var carsResult = await _carService.GetCarsByUserIdAsync(CurrentUser.UserId);
            if (carsResult.IsSuccessful)
            {
                var allUserCars = carsResult.Value.ToList();
                MyCars = allUserCars.Where(c => c.IsActive).OrderByDescending(c => c.CreatedAt).ToList();
                TotalAnnounced = allUserCars.Count(c => c.IsApproved);
            }

            // 3. Total de Favoritos (Garagem Privada)
            var favsResult = await _carService.GetFavoriteCarByUserIdAsync(CurrentUser.UserId);
            TotalGarage = favsResult.IsSuccessful ? favsResult.Value.Count() : 0;

            return Page();
        }
    }
}
