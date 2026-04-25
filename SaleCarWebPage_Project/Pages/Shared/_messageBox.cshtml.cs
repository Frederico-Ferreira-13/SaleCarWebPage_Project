using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SaleCarWebPage_Project.Pages.Shared
{
    public class _messageBoxModel : PageModel
    {
        public List<MessageBox> ChatHistory { get; set; } = new();

        public int CarId { get; set; }
        public int? ProviderId { get; set; }
        public string? NewMessageText { get; set; }
       
    }
}
