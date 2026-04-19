using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleCarWebPage_Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICarService _carService;
        private readonly ITokenService _tokenService;
        private readonly IBrandService _brandService;

        public IndexModel(ILogger<IndexModel> logger, ICarService carService, ITokenService tokenService, 
            IBrandService brandService)
        {
            _logger = logger;
            _carService = carService;
            _tokenService = tokenService;
            _brandService = brandService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? BrandId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Location { get; set; }

        public SelectList BrandList { get; set; }
        public List<Car> CarList { get; set; } = new List<Car>();      


        public async Task OnGetAsync()
        {
            var brandsResult = await _brandService.GetAllBrandsAsync();
            if (brandsResult.IsSuccessful)
            {
                // Criamos uma lista de seleção: DataValueField="BrandId", DataTextField="Name"
                BrandList = new SelectList(brandsResult.Value, "BrandId", "BrandName", BrandId);
            }

            try
            {
                var userIdResult = await _tokenService.GetUserIdFromContextAsync();
                int? userId = userIdResult.IsSuccessful ? userIdResult.Value : null;

                if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
                {
                    var pendingResult = await _carService.GetPendingApprovalAsync();
                    ViewData["PendingCount"] = pendingResult.IsSuccessful ? pendingResult.Value!.Count() : 0;
                }

                // 3. Carregar os carros para a Grelha (Destaques)
                // Aqui filtramos apenas os carros aprovados
                var searchResult = await _carService.SearchCarsAsync(
                    searchTerm: SearchTerm,
                    brandId: BrandId,
                    modelId: null,
                    fuelType: null,
                    page: 1,
                    pageSize: 12
                );

                if (searchResult.Items != null)
                {
                    CarList = searchResult.Items
                        .Where(c => c.IsApproved)
                        .OrderByDescending(c => c.CreatedAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a montra de veículos.");
            }
        }
    }    
}
