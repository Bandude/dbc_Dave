using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace dbc_Dave.Data.Models
{


        public class Model
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("object")]
            public string ObjectType { get; set; }

            [JsonProperty("created")]
            [JsonConverter(typeof(UnixDateTimeConverter))] // To convert Unix timestamp to DateTime
            public DateTime Created { get; set; }

            [JsonProperty("owned_by")]
            public string OwnedBy { get; set; }
        }

        public class ModelsList
        {
            [JsonProperty("object")]
            public string ObjectType { get; set; }

            [JsonProperty("data")]
            public List<Model> Data { get; set; }
        }
    
}
