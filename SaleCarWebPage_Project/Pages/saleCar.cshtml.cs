using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class SaleCarModel : PageModel
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;

        public SaleCarModel(ICarService carService, IBrandService brandService)
        {
            _carService = carService;
            _brandService = brandService;
        }

        [BindProperty]
        public Car NewCar { get; set; }

        public SelectList BrandList { get; set; }
        public SelectList FuelList { get; set; }
        public SelectList TransmissionList { get; set; }

        public async Task OnGetAsync()
        {
            await LoadListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // O serviço de criação deve tratar internamente do ProviderId, Data e Status
            // devido às restrições de acesso (setters privados) na classe Car.
            var result = await _carService.CreateCarAsync(NewCar);

            if (result.IsSuccessful)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            await LoadListsAsync();
            return Page();
        }

        private async Task LoadListsAsync()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            if (brands != null && brands.IsSuccessful)
            {
                BrandList = new SelectList(brands.Value, "BrandId", "BrandName");
            }

            FuelList = new SelectList(new List<string> { "Gasolina", "Diesel", "Híbrido", "Elétrico" });
            TransmissionList = new SelectList(new List<string> { "Automática", "Manual" });
        }
    }
}