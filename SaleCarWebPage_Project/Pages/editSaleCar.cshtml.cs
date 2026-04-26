using Contracts.Services;
using Core.Common;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class editSaleCarModel : PageModel
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly IUsersService _usersService;

        public editSaleCarModel(ICarService carService, IBrandService brandService, IUsersService usersService)
        {
            _carService = carService;
            _brandService = brandService;
            _usersService = usersService;
        }

        [BindProperty]
        public Car CarToEdit { get; set; }

        public SelectList BrandList { get; set; }
        public SelectList FuelList { get; set; }
        public SelectList TransmissionList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0) return RedirectToIndex("ID de veículo inválido.");
            
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? currentUserId = int.TryParse(currentUserIdClaim, out int parsedId) ? parsedId : null;

            var result = await _carService.GetCarByIdAsync(id, currentUserId);

            if (!result.IsSuccessful || result.Value == null)
                return RedirectToIndex("Veículo não encontrado.");

            var car = result.Value;
            
            var authCheck = await CheckUserPermissions(car.ProviderId);
            if (authCheck != null) return authCheck;
           
            CarToEdit = car;
            await LoadListsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }            

            var result = await _carService.UpdateCarAsync(id, CarToEdit);
            
            ModelState.AddModelError(string.Empty, "O método de atualização precisa de ser adicionado à ICarService.");
            await LoadListsAsync();
            return Page();
        }        

        private async Task<IActionResult?> CheckUserPermissions(int ownerProviderId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
                return RedirectToPage("/auth");
           
            var userResult = await _usersService.GetUserByIdAsync(currentUserId);
            var currentUser = userResult.Value;

            bool isAdmin = currentUser?.UsersRoleId == 1;            
            bool isOwner = ownerProviderId == currentUser?.UserId;

            if (!isAdmin && !isOwner)
            {
                TempData["ErrorMessage"] = "Acesso negado. Apenas o proprietário ou administradores podem editar.";
                return RedirectToPage("/Index");
            }

            return null;
        }

        private async Task LoadListsAsync()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            if (brands?.IsSuccessful == true)
                BrandList = new SelectList(brands.Value, "BrandId", "BrandName");

            FuelList = new SelectList(new List<string> { "Gasolina", "Diesel", "Híbrido", "Elétrico" });
            TransmissionList = new SelectList(new List<string> { "Automática", "Manual" });
        }

        private IActionResult RedirectToIndex(string message)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToPage("/Index");
        }
    }
}
