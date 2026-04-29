using Contracts.Services;
using Core.Common;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize(Roles = "1, Admin")]
    public class pendingCarModel : PageModel
    {
        private readonly ICarService _carService;
        private readonly IUsersService _userService;

        public pendingCarModel(ICarService carService, IUsersService userService)
        {
            _carService = carService;
            _userService = userService;
        }

        public List<Car> PendingCars { get; set; } = new();
        public List<Users> PendingUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            var carResult = await _carService.GetPendingApprovalAsync();
            if (carResult.IsSuccessful)
            {
                PendingCars = carResult.Value!.ToList();
            }

            var userResult = await _userService.GetPendingUsersAsync();
            if (userResult.IsSuccessful)
            {
                PendingUsers = userResult.Value!.ToList();
            }
        }

        public async Task<IActionResult> OnPostApproveCarAsync(int id)
        {
            var result = await _userService.ApproveUserAsync(id);
            StatusMessage<bool>(result, "Utilizador aprovado!");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveUserAsync(int id)
        {
            var result = await _userService.ApproveUserAsync(id);
            StatusMessage(result, "Utilizador aprovado com sucesso!");
            return RedirectToPage();
        }

        private void StatusMessage<T>(Result<T> result, string success)
        {
            if (result.IsSuccessful) TempData["SuccessMessage"] = success;
            else TempData["ErrorMessage"] = result.Message ?? "Erro na operação.";
        }
    }
}
