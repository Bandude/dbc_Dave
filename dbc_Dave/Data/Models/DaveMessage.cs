namespace dbc_Dave.Data.Models
{
    public class DaveMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public bool EditMode { get; set; }

        public DaveMessage(string role, string content)
        {
            Role = role;
            Content = content;
            EditMode = false;
        }
    }
}