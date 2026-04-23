using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages
{
    public class profileEditModel : PageModel
    {
        private readonly IUsersService _usersService;
        private readonly ICloudService _cloudService;

        public profileEditModel(IUsersService usersService, ICloudService cloudService)
        {
            _usersService = usersService;
            _cloudService = cloudService;
        }

        public Users CurrentUser { get; set; } = null!;

        [BindProperty]
        public IFormFile? PhotoUpload { get; set; }

        [BindProperty]
        public string InputUserName { get; set; } = string.Empty;

        [BindProperty]
        public string InputEmail { get; set; } = string.Empty;

        [BindProperty]
        public string? InputFacebook { get; set; }

        [BindProperty]
        public string? InputInstagram { get; set; }

        [BindProperty]
        public string? InputX { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userResult = await _usersService.GetCurrentUserAsync();

            if (!userResult.IsSuccessful || userResult.Value == null)
            {
                return RedirectToPage("/Login");
            }

            CurrentUser = userResult.Value!;

            InputUserName = CurrentUser.UserName;
            InputEmail = CurrentUser.Email;

            InputFacebook = CurrentUser.FacebookUrl;
            InputInstagram = CurrentUser.InstagramUrl;
            InputX = CurrentUser.TwitterUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("--- [LOG] Iniciando OnPostAsync ---");

            var userResult = await _usersService.GetCurrentUserAsync();
            if (!userResult.IsSuccessful || userResult.Value == null)
            {
                return RedirectToPage("/Login");
            }

            var userToUpdate = userResult.Value;

            // 1. Processar Foto
            if (PhotoUpload != null && PhotoUpload.Length > 0)
            {
                const long maxFileSize = 10485760;

                if (PhotoUpload.Length > maxFileSize)
                {
                    Console.WriteLine($"--- [LOG] Erro: Ficheiro demasiado grande ({PhotoUpload.Length} bytes) ---");
                    ModelState.AddModelError("PhotoUpload", "A imagem é demasiado grande. O limite máximo é de 10MB.");

                    // Recarrega os dados para a página não perder as informações atuais
                    CurrentUser = userToUpdate;
                    return Page();
                }

                try
                {
                    var imageUrl = await _cloudService.UploadImageAsync(PhotoUpload);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        Console.WriteLine($"--- [LOG] Sucesso Cloudinary: {imageUrl} ---");

                        // USAR O MÉTODO DA ENTIDADE EM VEZ DO "="
                        userToUpdate.UpdateProfilePicture(imageUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "O serviço de imagem não retornou um endereço válido.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--- [LOG] Erro CloudService: {ex.Message} ---");
                    ModelState.AddModelError(string.Empty, "Erro no upload da imagem.");
                }
            }

            // 2. Atualizar Nome e Email usando os métodos permitidos
            try
            {
                // USAR OS MÉTODOS QUE JÁ TENS EM VEZ DO "="
                userToUpdate.UpdateUserName(InputUserName);
                userToUpdate.UpdateEmail(InputEmail);

                userToUpdate.UpdateSocialLinks(InputFacebook, InputInstagram, InputX);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--- [LOG] Erro de validação: {ex.Message} ---");
                ModelState.AddModelError(string.Empty, ex.Message);
                CurrentUser = userToUpdate;
                return Page();
            }

            // 3. Gravar na Base de Dados
            var result = await _usersService.UpdateUserProfileAsync(userToUpdate);

            if (result.IsSuccessful)
            {
                TempData["Success"] = "Perfil atualizado com sucesso!";
                return RedirectToPage("/profile");
            }

            ModelState.AddModelError(string.Empty, "Erro ao gravar na base de dados.");
            CurrentUser = userToUpdate;
            return Page();
        }
    }
}
