using Newtonsoft.Json;

namespace dbc_Dave.Data.Models
{

    public class Assistant
    {
        public string Id { get; set; }
        public string Object { get; set; } = "assistant";
        public int CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string Instructions { get; set; }
        public List<Tool> Tools { get; set; } // Assuming Tool is a base class for specific tool types
        public List<string> FileIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

    }
    public class AssistantList
    {
        [JsonProperty("object")]
        public string ObjectType { get; set; }
        [JsonProperty("data")]
        public List<Assistant> Data { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        [JsonProperty("first_id")]
        public string FirstId { get; set; } // Corrected type to string
        [JsonProperty("last_id")]
        public string LastId { get; set; } // Corrected type to string
    }

    public class AssistantFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string ObjectType { get; set; }

        // Additional properties can be included here as needed
    }

    public class AssistantFileDeleted
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string ObjectType { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
    }
}