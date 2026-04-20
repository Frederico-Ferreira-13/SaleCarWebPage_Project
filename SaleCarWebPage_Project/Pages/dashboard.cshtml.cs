using Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class dashboardModel : PageModel
    {
        private readonly ICarService _carService;
        private readonly ISaleService _saleService;

        public dashboardModel(ICarService carService, ISaleService saleService)
        {
            _carService = carService;
            _saleService = saleService;
        }

        public DashboardAdminDto AdminData { get; set; } = new();
        public DashboardUserDto UserData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToPage("/auth");

            int userId = int.Parse(userIdClaim);

            if (User.IsInRole("Admin"))
            {
                // No futuro, estes valores virão de métodos agregados no Repositório/Service
                // Por agora, simulamos a lógica estatística
                AdminData = new DashboardAdminDto
                {
                    TotalCarsSold = 24, // Ex: _saleService.GetAllSalesCount()
                    TopBrand = "Porsche",
                    TopModel = "911 GT3"
                };
            }
            else
            {
                // 1. Obter carros que o usuário ainda tem à venda
                var activeCars = await _carService.GetCarsByUserIdAsync(userId);

                // 2. Obter inventário de vendidos através do ISaleService
                var soldCars = await _saleService.GetSoldInventoryAsync(userId);

                UserData = new DashboardUserDto
                {
                    CarsSoldByUser = soldCars.Value?.Count() ?? 0,
                    CarsForSaleByUser = activeCars.Value?.Count(c => c.IsAvailable) ?? 0,

                    // Simulação de propostas (Aqui podes ligar ao teu IMessageService mais tarde)
                    CarProposals = activeCars.Value?.Select(c => new CarProposalDto
                    {
                        CarName = $"{c.Model?.Brand?.BrandName} {c.Model?.ModelName}",
                        ProposalCount = 3 // Valor exemplo
                    }).ToList() ?? new List<CarProposalDto>()
                };
            }

            return Page();
        }

        // DTOs integrados para facilitar o acesso na View
        public class DashboardAdminDto
        {
            public int TotalCarsSold { get; set; }
            public string TopBrand { get; set; }
            public string TopModel { get; set; }
        }

        public class DashboardUserDto
        {
            public int CarsSoldByUser { get; set; }
            public int CarsForSaleByUser { get; set; }
            public List<CarProposalDto> CarProposals { get; set; } = new();
        }

        public class CarProposalDto
        {
            public string CarName { get; set; }
            public int ProposalCount { get; set; }
        }
    }
}

