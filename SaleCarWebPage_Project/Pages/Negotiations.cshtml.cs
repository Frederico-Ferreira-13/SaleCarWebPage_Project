using Contracts.Services;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SaleCarWebPage_Project.Pages.Shared;

namespace SaleCarWebPage_Project.Pages
{
    [Authorize]
    public class NegotiationsModel : PageModel
    {
        private readonly ISaleService _saleService;
        private readonly ICarService _carService;
        private readonly ITokenService _tokenService;
        private readonly IMessageBoxService _messageService;

        public NegotiationsModel(ISaleService saleService, ICarService carService, ITokenService tokenService,
            IMessageBoxService messageService)
        {
            _saleService = saleService;
            _carService = carService;
            _tokenService = tokenService;
            _messageService = messageService;
        }

        public List<NegotiationItemViewModel> Negotiations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? carId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return RedirectToPage("/auth");
            int currentUserId = userIdResult.Value;

            var allProposalsResult = await _saleService.GetAllAsync(); // Ou um método mais específico
            if (!allProposalsResult.IsSuccessful) return Page();

            var userNegotiations = allProposalsResult.Value
                .Where(p => p.ClientId == currentUserId || p.CarId != 0) // Ajustar lógica conforme a tua BD
                .GroupBy(p => p.CarId);

            foreach (var group in userNegotiations)
            {
                int idCarro = group.Key;
                var carResult = await _carService.GetCarByIdAsync(idCarro, currentUserId);
                if (!carResult.IsSuccessful) continue;

                var car = carResult.Value;
                var lastProposal = group.OrderByDescending(p => p.SaleDate).First();

                // Procurar histórico de mensagens para este carro
                var chatResult = await _messageService.GetChatHistoryAsync(idCarro, currentUserId);

                var item = new NegotiationItemViewModel
                {
                    SaleId = lastProposal.SaleId,
                    CarId = idCarro,
                    CarName = $"{car.Model?.Brand?.BrandName} {car.Model?.ModelName}",
                    CarImage = car.ImageUrl ?? "/images/cars/default.jpg",
                    OtherPartyName = car.Provider?.User?.Name ?? "Vendedor",
                    LastOfferValue = lastProposal.FinalPrice,
                    IsAccepted = false, // Podes adicionar lógica de estado na entidade Sale

                    ProposalData = new _proposalBoxModel
                    {
                        CarId = idCarro,
                        CurrentUserId = currentUserId,
                        IsSellerView = car.Provider?.UserId == currentUserId,
                        ProposalHistory = group.OrderByDescending(p => p.SaleDate).ToList(),
                        ReadOnly = false,
                        OtherPartyName = car.Provider?.UserId == currentUserId ? "Cliente" : car.Provider?.User?.Name
                    },

                    MessageBoxData = new _messageBoxModel
                    {
                        CarId = idCarro,
                        ChatHistory = chatResult.IsSuccessful ? chatResult.Value.ToList() : new List<MessageBox>(),
                        OtherPartyName = car.Provider?.UserId == currentUserId ? "Cliente" : car.Provider?.User?.Name,
                        ReadOnly = false
                    }
                };

                Negotiations.Add(item);
            }

            // Se um carId foi passado, garantimos que essa negociação aparece primeiro
            if (carId.HasValue)
            {
                Negotiations = Negotiations.OrderByDescending(n => n.CarId == carId.Value).ToList();
            }

            return Page();
        }

        public class NegotiationItemViewModel
        {
            public int SaleId { get; set; }
            public int CarId { get; set; }
            public string CarName { get; set; }
            public string CarImage { get; set; }
            public string OtherPartyName { get; set; }
            public decimal LastOfferValue { get; set; }
            public bool IsAccepted { get; set; }
            public bool IsRead { get; set; }

            // Propriedades para as Partials
            public _proposalBoxModel ProposalData { get; set; } = new();

            public _messageBoxModel MessageBoxData { get; set; } = new();

            public List<Core.Model.Sale> Offers => ProposalData.ProposalHistory;
        }
    }    
}
