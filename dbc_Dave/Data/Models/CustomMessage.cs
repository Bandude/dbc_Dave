namespace dbc_Dave.Data.Models
{
    public class CustomMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public bool EditMode { get; set; }

        public CustomMessage(string role, string content)
        {
            Role = role;
            Content = content;
            EditMode = false;
        }
    }
}