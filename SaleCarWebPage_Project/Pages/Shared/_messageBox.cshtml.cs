using Core.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages.Shared
{
    public class _messageBoxModel : PageModel
    {
        public int CarId { get; set; }
        public int? ProviderId { get; set; }
        public List<MessageBox> ChatHistory { get; set; } = new();
        public bool ReadOnly { get; set; } = false;
        public Dictionary<int, string> UserNames { get; set; } = new();
    }
}