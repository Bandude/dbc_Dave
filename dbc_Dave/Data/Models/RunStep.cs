using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace dbc_Dave.Data.Models;

public class RunStep
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string ObjectType { get; set; } = "thread.run.step";

    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("assistant_id")]
    public string AssistantId { get; set; }

    [JsonPropertyName("thread_id")]
    public string ThreadId { get; set; }

    [JsonPropertyName("run_id")]
    public string RunId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("step_details")]
    public StepDetails StepDetails { get; set; }

    [JsonPropertyName("last_error")]
    public LastError LastError { get; set; } = null;

    [JsonPropertyName("expires_at")]
    public int? ExpiredAt { get; set; } = null;

    [JsonPropertyName("cancelled_at")]
    public int? CancelledAt { get; set; } = null;

    [JsonPropertyName("failed_at")]
    public int? FailedAt { get; set; } = null;

    [JsonPropertyName("completed_at")]
    public int? CompletedAt { get; set; } = null;

    [JsonExtensionData]
    public Dictionary<string, object> Metadata { get; set; }

    public RunStep()
    {
        Metadata = new Dictionary<string, object>();
    }
}

public class StepDetails
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("message_creation")]
    public MessageCreationDetails MessageCreation { get; set; }
}

public class MessageCreationDetails
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
}

public class LastError
{
    // Assuming we have properties here with JsonProperty decorators.
    // This is just a placeholder as the specific structure is not provided.
}

public class StepList
{
    [JsonPropertyName("object")]
    public string ObjectType { get; set; }

    [JsonPropertyName("data")]
    public List<Run> data { get; set; }
}