namespace SOS_Alert_App.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public ICollection<AlertLog> AlertLogs { get; set; } = new List<AlertLog>();

    }
}
