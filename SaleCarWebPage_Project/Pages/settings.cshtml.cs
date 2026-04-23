using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SaleCarWebPage_Project.Pages
{
    public class settingsModel : PageModel
    {
        private readonly IUsersService _usersService;

        public settingsModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova password deve ter pelo menos 6 caracteres.")]
        public string? NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As passwords não coincidem.")]
        public string? ConfirmPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userResult = await _usersService.GetCurrentUserAsync();
            if (!userResult.IsSuccessful || userResult.Value == null)
                return RedirectToPage("/Login");

            Email = userResult.Value.Email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Login");
            
            var userResult = await _usersService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccessful || userResult.Value == null)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado.");
                return Page();
            }

            var userToUpdate = userResult.Value;

            try
            {
                // Verifica se o email mudou antes de atualizar
                if (!string.Equals(userToUpdate.Email, Email, StringComparison.OrdinalIgnoreCase))
                {
                    userToUpdate.UpdateEmail(Email);
                }

                // Se decidires manter Nome/Username aqui também:
                // userToUpdate.UpdateName(Name);
                // userToUpdate.UpdateUserName(UserName);

                var updateProfileResult = await _usersService.UpdateUserProfileAsync(userToUpdate);
                if (!updateProfileResult.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, updateProfileResult.Message ?? "Erro ao atualizar perfil.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (string.IsNullOrWhiteSpace(CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Deve introduzir a password atual para definir uma nova.");
                    return Page();
                }

                var passwordResult = await _usersService.ChangeUserPasswordAsync(userId, CurrentPassword, NewPassword);
                if (!passwordResult.IsSuccessful)
                {
                    ModelState.AddModelError("CurrentPassword", passwordResult.Message ?? "A password atual está incorreta.");
                    return Page();
                }
            }           

            ModelState.AddModelError(string.Empty, "Erro ao atualizar as definições na base de dados.");
            return Page();
        }

        private int GetUserId()
        {           
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
