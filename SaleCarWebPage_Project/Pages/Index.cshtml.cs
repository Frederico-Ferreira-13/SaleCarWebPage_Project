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
        [BindProperty(SupportsGet = true)] public string? CarType { get; set; }
        [BindProperty(SupportsGet = true)] public string? FuelTypeSelected { get; set; }
        [BindProperty(SupportsGet = true)] public string? Transmission { get; set; }

        public SelectList BrandList { get; set; }
        public SelectList TypeList { get; set; }
        public SelectList FuelList { get; set; }
        public SelectList TransmissionList { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();

        public async Task OnGetAsync()
        {
            _logger.LogInformation("--- DEBUG FILTROS ---");
            _logger.LogInformation($"Search: {SearchTerm} | Fuel: {FuelTypeSelected} | Trans: {Transmission}");

            try
            {
                // 1. Carregar Listas para os Dropdowns
                var brandsResult = await _brandService.GetAllBrandsAsync();
                BrandList = (brandsResult.IsSuccessful && brandsResult.Value != null)
                    ? new SelectList(brandsResult.Value, "BrandId", "BrandName", BrandId)
                    : GerarMarcasMock();

                TypeList = new SelectList(new List<string> { "Citadino", "Desportivo", "SUV", "Sedan", "Carrinha", "Cabriolet" }, CarType);
                FuelList = new SelectList(new List<string> { "Gasolina", "Diesel", "Híbrido", "Elétrico" }, FuelTypeSelected);
                TransmissionList = new SelectList(new List<string> { "Manual", "Automática" }, Transmission);

                // 2. Chamada ao serviço (Usamos o FuelTypeSelected aqui pois a coluna na BD é TypeOfFuel)
                var searchResult = await _carService.SearchCarsAsync(
                    searchTerm: SearchTerm,
                    brandId: BrandId,
                    modelId: null,
                    fuelType: FuelTypeSelected, // Agora passamos o combustível real
                    page: 1,
                    pageSize: 50
                );

                if (searchResult.Items != null)
                {
                    var tempCars = searchResult.Items
                        .Where(c => c.IsApproved && c.IsActive)
                        .ToList();

                    // --- FILTRO DE TEXTO MANUAL (Caso o SQL LIKE falhe como no log) ---
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        tempCars = tempCars.Where(c =>
                            (c.Model != null && c.Model.ModelName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                            (c.PlateNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                    // --- FILTRO DE MARCA MANUAL ---
                    if (BrandId.HasValue && BrandId.Value > 0)
                    {
                        tempCars = tempCars.Where(c => c.Model != null && c.Model.BrandId == BrandId.Value).ToList();
                    }

                    // --- FILTRO DE LOCALIZAÇÃO MANUAL ---
                    if (!string.IsNullOrEmpty(Location))
                    {
                        tempCars = tempCars.Where(c =>
                            (c.Provider?.Address?.City != null && c.Provider.Address.City.Equals(Location, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                    if (!string.IsNullOrEmpty(Transmission))
                    {
                        tempCars = tempCars.Where(c =>
                            c.Transmission != null && c.Transmission.Equals(Transmission, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    }

                    if (!string.IsNullOrEmpty(CarType))
                    {
                        tempCars = tempCars.Where(c =>
                            c.Category != null && c.Category.Equals(CarType, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    }

                    // 3. Favoritos
                    var userIdResult = await _tokenService.GetUserIdFromContextAsync();
                    int currentUserId = userIdResult.IsSuccessful ? userIdResult.Value : 0;

                    if (currentUserId > 0)
                    {
                        foreach (var car in tempCars)
                        {
                            car.IsFavorite = await _carService.IsCarFavoriteAsync(car.CarId, currentUserId);
                        }
                    }

                    Cars = tempCars.OrderByDescending(c => c.CreatedAt).ToList();
                }            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar coleção.");
                BrandList = GerarMarcasMock(); 
            }                        
        }

        private SelectList GerarMarcasMock()
        {
            var marcasFake = new List<object> {
                new { BrandId = 1, BrandName = "Ferrari" },
                new { BrandId = 2, BrandName = "Porsche" },
                new { BrandId = 3, BrandName = "Lamborghini" },
                new { BrandId = 4, BrandName = "Aston Martin" },
                new { BrandId = 5, BrandName = "McLaren" }
            };
            return new SelectList(marcasFake, "BrandId", "BrandName", BrandId);
        }
    }
}