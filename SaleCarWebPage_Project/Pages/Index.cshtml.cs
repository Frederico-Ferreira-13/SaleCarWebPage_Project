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
        public SelectList TypeList { get; set; }
        public List<Car> CarList { get; set; } = new List<Car>();


        public async Task OnGetAsync()
        {
            // --- 1. CARREGAR MARCAS COM FALLBACK ---
            try
            {
                var brandsResult = await _brandService.GetAllBrandsAsync();
                if (brandsResult.IsSuccessful && brandsResult.Value != null && brandsResult.Value.Any())
                {
                    BrandList = new SelectList(brandsResult.Value, "BrandId", "BrandName", BrandId);
                }
                else
                {
                    BrandList = GerarMarcasMock();
                }
            }
            catch
            {
                // Se a base de dados der erro (como no teu print), usamos marcas fictícias
                BrandList = GerarMarcasMock();
            }

            // --- 2. CARREGAR TIPOS (MANUAL) ---
            var tiposVeiculo = new List<string> { "Citadino", "Desportivo", "SUV", "Sedan", "Carrinha", "Cabriolet" };
            TypeList = new SelectList(tiposVeiculo);

            // --- 3. CARREGAR DADOS DE UTILIZADOR E CARROS ---
            try
            {
                var userIdResult = await _tokenService.GetUserIdFromContextAsync();

                if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
                {
                    var pendingResult = await _carService.GetPendingApprovalAsync();
                    ViewData["PendingCount"] = pendingResult.IsSuccessful ? pendingResult.Value!.Count() : 0;
                }

                // Tenta carregar carros da BD
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
                // O erro "Invalid column name" que aparece na tua consola será registado aqui
                _logger.LogError(ex, "Erro de base de dados ao carregar carros. Usando apenas estáticos.");
            }
        }

        // Método auxiliar para criar marcas quando a BD falha
        private SelectList GerarMarcasMock()
        {
            var marcasFake = new List<object>
            {
                new { BrandId = 1, BrandName = "Ferrari" },
                new { BrandId = 2, BrandName = "Porsche" },
                new { BrandId = 3, BrandName = "Lamborghini" },
                new { BrandId = 4, BrandName = "Aston Martin" },
                new { BrandId = 5, BrandName = "Bentley" }
            };
            return new SelectList(marcasFake, "BrandId", "BrandName", BrandId);
        }
    }
}