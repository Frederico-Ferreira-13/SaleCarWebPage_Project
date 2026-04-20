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

        public IndexModel(ILogger<IndexModel> logger, ICarService carService, ITokenService tokenService, IBrandService brandService)
        {
            _logger = logger;
            _carService = carService;
            _tokenService = tokenService;
            _brandService = brandService;
        }

        [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)] public int? BrandId { get; set; }
        [BindProperty(SupportsGet = true)] public string? Location { get; set; }

        public SelectList BrandList { get; set; }
        public SelectList TypeList { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();

        public async Task OnGetAsync()
        {
            // 1. Carregar Marcas e Tipos
            try
            {
                var brandsResult = await _brandService.GetAllBrandsAsync();
                BrandList = (brandsResult.IsSuccessful && brandsResult.Value != null)
                    ? new SelectList(brandsResult.Value, "BrandId", "BrandName", BrandId)
                    : GerarMarcasMock();
            }
            catch { BrandList = GerarMarcasMock(); }

            TypeList = new SelectList(new List<string> { "Citadino", "Desportivo", "SUV", "Sedan", "Carrinha", "Cabriolet" });

            // 2. Carregar Carros e Favoritos
            try
            {
                var userIdResult = await _tokenService.GetUserIdFromContextAsync();
                int currentUserId = userIdResult.IsSuccessful ? userIdResult.Value : 0;

                if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
                {
                    var pendingResult = await _carService.GetPendingApprovalAsync();
                    ViewData["PendingCount"] = pendingResult.IsSuccessful ? pendingResult.Value!.Count() : 0;
                }

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
                    var tempCars = searchResult.Items
                        .Where(c => c.IsApproved)
                        .OrderByDescending(c => c.CreatedAt)
                        .ToList();

                    // 3. Lógica de Favoritos corrigida (O método retorna bool diretamente)
                    if (currentUserId > 0)
                    {
                        foreach (var car in tempCars)
                        {
                            // CORREÇÃO: Removido .IsSuccessful e .Value pois o retorno é bool
                            bool isFav = await _carService.IsCarFavoriteAsync(car.CarId, currentUserId);

                            // Define a propriedade (Certifica-te que 'IsFavorite' existe no teu Car.cs)
                            car.IsFavorite = isFav;
                        }
                    }
                    Cars = tempCars;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar coleção.");
                Cars = new List<Car>();
            }
        }

        private SelectList GerarMarcasMock()
        {
            var marcasFake = new List<object> {
                new { BrandId = 1, BrandName = "Ferrari" },
                new { BrandId = 2, BrandName = "Porsche" },
                new { BrandId = 3, BrandName = "Lamborghini" }
            };
            return new SelectList(marcasFake, "BrandId", "BrandName", BrandId);
        }
    }
}