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
        public Car CarToEdit { get; set; } = default!;

        public SelectList BrandList { get; set; } = default!;
        // Nota: FuelList e TransmissionList são usados para popular os rádios e selects no HTML
        public SelectList FuelList { get; set; } = default!;
        public SelectList TransmissionList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0) return RedirectToIndex("ID de veículo inválido.");

            // Obtemos o carro sem filtro de utilizador inicial para validar permissões manualmente
            var result = await _carService.GetCarByIdAsync(id, null);

            if (!result.IsSuccessful || result.Value == null)
                return RedirectToIndex("Veículo não encontrado.");

            var car = result.Value;

            // Verifica se o utilizador tem permissão (Dono ou Admin Role 1)
            var authCheck = await CheckUserPermissions(car.ProviderId);
            if (authCheck != null) return authCheck;

            CarToEdit = car;
            await LoadListsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // 1. Re-verificação de segurança no Post
            var carCheck = await _carService.GetCarByIdAsync(id, null);
            if (!carCheck.IsSuccessful || carCheck.Value == null)
                return RedirectToIndex("Veículo não encontrado.");

            var authCheck = await CheckUserPermissions(carCheck.Value.ProviderId);
            if (authCheck != null) return authCheck;

            // 2. Validação do ModelState
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            // 3. Persistência das alterações
            var result = await _carService.UpdateCarAsync(id, CarToEdit);

            if (result.IsSuccessful)
            {
                TempData["SuccessMessage"] = "Anúncio atualizado com sucesso!";
                // Redireciona para a página de detalhes do carro
                return RedirectToPage("/viewCar", new { id = id });
            }

            // Tratamento de erro vindo do serviço (ex: erro de base de dados)
            ModelState.AddModelError(string.Empty, result.Message ?? "Erro ao atualizar o veículo.");
            await LoadListsAsync();
            return Page();
        }

        /// <summary>
        /// Centraliza a lógica de permissões: Admin (Role 1) ou o próprio Dono do anúncio.
        /// </summary>
        private async Task<IActionResult?> CheckUserPermissions(int ownerProviderId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
                return RedirectToPage("/auth");

            var userResult = await _usersService.GetUserByIdAsync(currentUserId);
            var currentUser = userResult.Value;

            // Lógica: Se for Role 1 (Admin) ou se o ID do logado for igual ao Dono do carro
            bool isAdmin = currentUser?.UsersRoleId == 1;
            bool isOwner = ownerProviderId == currentUserId;

            if (!isAdmin && !isOwner)
            {
                TempData["ErrorMessage"] = "Acesso negado. Apenas o proprietário ou administradores podem editar.";
                return RedirectToPage("/Index");
            }

            return null;
        }

        private async Task LoadListsAsync()
        {
            // Carrega marcas da BD
            var brands = await _brandService.GetAllBrandsAsync();
            if (brands?.IsSuccessful == true)
                BrandList = new SelectList(brands.Value, "BrandId", "BrandName");

            // Define listas estáticas para os seletores
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