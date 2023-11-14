using dbc_Dave.Services;

namespace dbc_Dave.Data.Models
{
    // Model classes for the JSON responses.
    public class AssistantThread
    {
        public List<Message> messages { get; set; }
    }
    public class ThreadResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public Metadata Metadata { get; set; }
    }

    public class DeleteThreadResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public bool Deleted { get; set; }
    }

    public class ModifyThreadRequest
    {
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        public string Modified { get; set; }
        public string User { get; set; }
    }

    public class ThreadAndRun
    {

        public string assistant_id { get; set; }
        public AssistantThread thread { get; set; }
        
    }


}
