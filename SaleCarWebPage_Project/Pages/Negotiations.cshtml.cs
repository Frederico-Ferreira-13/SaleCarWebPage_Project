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

        public List<NegotiationGroup> Negotiations { get; set; } = new();

        public class NegotiationGroup
        {
            public int CarId { get; set; }
            public string CarName { get; set; } = string.Empty;
            public string CarImage { get; set; } = "/img/cars/default-car.jpg";
            public string OtherPartyName { get; set; } = "Utilizador";
            public decimal LastOfferValue { get; set; }
            public bool IsSeller { get; set; }
            public DateTime LastActivity { get; set; }

            public _proposalBoxModel ProposalData { get; set; } = new();
            public _messageBoxModel MessageBoxData { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync(int? carId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) 
                return RedirectToPage("/auth");

            int currentUserId = userIdResult.Value;

            var myCarsResult = await _carService.GetCarsByUserIdAsync(currentUserId);
            var myCarIds = myCarsResult.IsSuccessful
                ? myCarsResult.Value.Select(c => c.CarId).ToHashSet()
                : new HashSet<int>();

            var negotiationsResult = await _saleService.GetUserNegotiationsAsync(currentUserId, myCarIds);
            if (!negotiationsResult.IsSuccessful || !negotiationsResult.Value.Any())
            {
                // Debug
                Console.WriteLine("[DEBUG] Nenhuma proposta encontrada para o utilizador.");
                return Page();
            }

            var grouped = negotiationsResult.Value
                .GroupBy(s => s.CarId);

            foreach (var group in grouped)
            {
                int carIdGrupo = group.Key;

                var carResult = await _carService.GetCarByIdAsync(carIdGrupo, currentUserId);
                if (!carResult.IsSuccessful || carResult.Value == null) continue;

                var car = carResult.Value;
                var lastProposal = group.OrderByDescending(p => p.SaleDate).First();

                bool isSeller = myCarIds.Contains(carIdGrupo);

                var otherPartyName = isSeller
                    ? (lastProposal.Client?.ClientName ?? lastProposal.Client?.User?.UserName ?? "Comprador")
                    : (car.Provider?.NameProvider ?? "Vendedor");

                var chatResult = await _messageService.GetChatHistoryAsync(carIdGrupo, currentUserId);

                var item = new NegotiationGroup
                {
                    CarId = carIdGrupo,
                    CarName = $"{car.Model?.Brand?.BrandName} {car.Model?.ModelName}",
                    CarImage = car.ImageUrl ?? "/img/cars/default-car.jpg",
                    OtherPartyName = otherPartyName,
                    LastOfferValue = lastProposal.FinalPrice,
                    IsSeller = isSeller,
                    LastActivity = lastProposal.SaleDate,

                    ProposalData = new _proposalBoxModel
                    {
                        CarId = carIdGrupo,
                        CurrentUserId = currentUserId,
                        IsSellerView = isSeller,
                        ProposalHistory = group.OrderByDescending(p => p.SaleDate).ToList(),
                        ReadOnly = false,
                        OtherPartyName = otherPartyName
                    },

                    MessageBoxData = new _messageBoxModel
                    {
                        CarId = carIdGrupo,
                        ChatHistory = chatResult.IsSuccessful ? chatResult.Value.ToList() : new(),
                        OtherPartyName = otherPartyName,
                        ReadOnly = false
                    }
                };

                Negotiations.Add(item);
            }

            Negotiations = Negotiations
                .OrderByDescending(n => n.LastActivity)
                .ToList();

            if (carId.HasValue)
            {
                var selected = Negotiations.FirstOrDefault(n => n.CarId == carId.Value);
                if (selected != null)
                {
                    Negotiations.Remove(selected);
                    Negotiations.Insert(0, selected);
                }
            }

            Console.WriteLine($"[DEBUG] Negociações carregadas: {Negotiations.Count}");
            return Page();
        }

        public async Task<IActionResult> OnPostAcceptAsync(int saleId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            var result = await _saleService.AcceptProposalAsync(saleId, userIdResult.Value);

            if (result.IsSuccessful)
                TempData["Success"] = "Proposta aceite com sucesso!";
            else
                TempData["Error"] = result.Message ?? "Erro ao aceitar proposta.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeclineAsync(int saleId)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            var result = await _saleService.DeclineProposalAsync(saleId, userIdResult.Value);

            if (result.IsSuccessful)
                TempData["Success"] = "Proposta recusada.";
            else
                TempData["Error"] = result.Message ?? "Erro ao recusar proposta.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCounterOfferAsync(int saleId, decimal counterValue)
        {
            var userIdResult = await _tokenService.GetUserIdFromContextAsync();
            if (!userIdResult.IsSuccessful) return Unauthorized();

            var result = await _saleService.CreateCounterOfferAsync(saleId, userIdResult.Value, counterValue);

            if (result.IsSuccessful)
                TempData["Success"] = $"Contra-proposta de {counterValue:C0} enviada!";
            else
                TempData["Error"] = result.Message ?? "Erro ao criar contra-proposta.";

            return RedirectToPage();
        }
    }    
}
