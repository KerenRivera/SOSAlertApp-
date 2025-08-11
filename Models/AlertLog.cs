namespace SOS_Alert_App.Models
{
    public class AlertLog
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; } = default!;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } 
    }
}
