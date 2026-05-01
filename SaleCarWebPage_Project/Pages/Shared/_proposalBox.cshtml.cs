using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages.Shared
{
    public class _proposalBoxModel : PageModel
    {
        public int CarId { get; set; }
        public int? CurrentUserId { get; set; }        
        public string OtherPartyName { get; set; } = string.Empty;        
        public List<Core.Model.Sale> ProposalHistory { get; set; } = new();        
        public bool IsSellerView { get; set; }
        public bool ReadOnly { get; set; }
    }
}
