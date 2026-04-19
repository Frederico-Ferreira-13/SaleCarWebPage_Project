using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Common;
using Core.Model;
using Contracts.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaleCarWebPage_Project.Pages
{
    public class authModel : PageModel
    {
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;

        public authModel(IUsersService usersService, ITokenService tokenService)
        {
            _usersService = usersService;
            _tokenService = tokenService;
        }

        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = new();

        public class LoginInputModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new();

        public class RegisterInputModel
        {
            public string Name { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostLoginAsync(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(LoginInput.Email) || string.IsNullOrEmpty(LoginInput.Password))
            {
                ModelState.AddModelError(string.Empty, "Preencha todos os campos de login.");
                return Page();
            }

            var authResult = await _usersService.AuthenticateUserAsync(LoginInput.Email, LoginInput.Password);

            if (!authResult.IsSuccessful || authResult.Value == null)
            {
                ModelState.AddModelError(string.Empty, authResult.Message ?? "Credenciais inválidas.");
                return Page();
            }

            if (!user.IsApproved)
            {
                ModelState.AddModelError(string.Empty, "A sua conta aguarda aprovação de um administrador.");
                return Page();
            }

            var user = authResult.Value;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UsersRoleId == 1 ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl ?? "/Index");
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _usersService.RegisterUserAsync(
                RegisterInput.UserName,
                RegisterInput.Name,
                RegisterInput.Email,
                RegisterInput.Password);

            if (result.IsSuccessful)
            {
                TempData["Success"] = "Conta criada! Faça login agora.";
                return RedirectToPage("/login"); // Recarrega para limpar os campos
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Erro ao registar.");
            return Page();
        }
    }
}
