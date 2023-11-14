using dbc_Dave.Services;

namespace dbc_Dave.Data.Models
{
    // Model classes for handling messages
    public class CreateMessageRequest
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class ModifyMessageRequest
    {
        public Metadata Metadata { get; set; }
    }

    public class MessageResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public string ThreadId { get; set; }
        public string Role { get; set; }
        public List<ContentItem> Content { get; set; }
        public List<string> FileIds { get; set; }
        public string AssistantId { get; set; }
        public string RunId { get; set; }
        public Metadata Metadata  { get; set; }
    }

    public class ContentItem
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }

    public class TextContent
    {
        public string Value { get; set; }
        public List<Annotation> Annotations { get; set; }
    }

    public class Annotation
    {
        // Define annotations if necessary...
    }

    public class MessageListResponse
    {
        public string Object { get; set; }
        public List<MessageResponse> Data { get; set; }
        public string FirstId { get; set; }
        public string LastId { get; set; }
        public bool HasMore { get; set; }
    }
}
