using dbc_Dave.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;



namespace dbc_Dave.Data.Models
{
    public abstract class Tool
    {
        [JsonProperty("type")] // Using Newtonsoft.Json attributes for serialization
        public string Type { get; protected set; } // Make the setter protected to prevent modification outside of the class inheritance chain
    }
    public class CodeInterpreterTool : Tool
    {
        public CodeInterpreterTool()
        {
            Type = "code_interpreter";
        }
        // Properties specific to CodeInterpreterTool
    }
    public class RetrievalTool : Tool
    {
        public RetrievalTool()
        {
            Type = "retrieval";
        }
        // Properties specific to RetrievalTool
    }
    public class FunctionTool : Tool
    {
        public FunctionTool()
        {
            Type = "function";
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
        public string Type { get; set; }
        public Dictionary<string, ParameterProperty> Properties { get; set; }
        public List<string> Required { get; set; }
    }
    public class ParameterProperty
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
}

public class ToolConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(Tool).IsAssignableFrom(objectType); // check if objectType is Tool or subtype
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        Tool tool = jsonObject["type"].Value<string>() switch
        {
            "code_interpreter" => new CodeInterpreterTool(),
            "retrieval" => new RetrievalTool(),
            "function" => new FunctionTool(),
            _ => throw new JsonException("Unknown tool type.")
        };

        serializer.Populate(jsonObject.CreateReader(), tool);
        return tool;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var tool = value as Tool;
        if (tool == null)
            throw new JsonSerializationException("Expected Tool object value");

        var jsonObject = JObject.FromObject(value, serializer); // Use the existing serializer to serialize the tool
        jsonObject.WriteTo(writer);
    }

    public override bool CanWrite => true;
}