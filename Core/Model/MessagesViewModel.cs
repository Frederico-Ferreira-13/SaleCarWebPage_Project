namespace Core.Model
{
    public class MessagesViewModel
    {
        public int CarId { get; set; }
        public int ProviderId { get; set; }
        public List<MessageBox> ChatHistory { get; set; } = new();
    }
}