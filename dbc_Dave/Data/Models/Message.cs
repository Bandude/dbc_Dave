using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace dbc_Dave.Data.Models
{

    public class Message
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string ObjectType { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("thread_id")]
        public string ThreadId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<MessageContent> Content { get; set; }

        [JsonPropertyName("file_ids")]
        public List<string> FileIds { get; set; }

        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }

        [JsonPropertyName("run_id")]
        public string RunId { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }


    public abstract class MessageContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

    }

    public class TextContent : MessageContent
    {
        [JsonPropertyName("text")]
        public TextData Text { get; set; }

        public TextContent()
        {
            Type = "text";
        }
    }

    public class ImageFileContent : MessageContent
    {
        [JsonPropertyName("image_file")]
        public ImageFileData ImageFile { get; set; }

        public ImageFileContent()
        {
            Type = "image_file";
        }
    }

    public class PolymorphicConverter<T> : JsonConverter<T> where T : MessageContent
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = document.RootElement;

                JsonElement typeElement;
                if (root.TryGetProperty("type", out typeElement))
                {
                    var type = typeElement.GetString();
                    switch (type)
                    {
                        case "text":
                            return JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                        case "image_file":
                            return JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                        default:
                            throw new JsonException("Unknown type.");
                    }
                }
            }
            throw new JsonException("Type property not found.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException("Use default serialization.");
        }
    }

    public class TextData
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("annotations")]
        public List<object> Annotations { get; set; }
    }

    public class ImageFileData
    {
        [JsonPropertyName("file_id")]
        public string FileId { get; set; }
    }

    public static class MessageRole
    {
        public const string User = "user";
        public const string Assistant = "assistant";
    }


    // Model classes for handling messages
    public class CreateMessageRequest
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Messages
    {
        List<CreateMessageRequest> CreateMessages { get; set; }
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
        public Metadata Metadata { get; set; }
    }

    public class ContentItem
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
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
