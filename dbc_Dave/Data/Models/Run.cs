using System;
using System.Collections.Generic;
using dbc_Dave.Data.Models;
using Newtonsoft.Json;


public class Run
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created_at")]
    public int CreatedAt { get; set; }

    [JsonProperty("run_id")]
    public string RunId { get; set; }

    [JsonProperty("assistant_id")]
    public string AssistantId { get; set; }

    [JsonProperty("thread_id")]
    public string ThreadId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("cancelled_at")]
    public int? CancelledAt { get; set; }

    [JsonProperty("completed_at")]
    public int? CompletedAt { get; set; }

    [JsonProperty("expires_at")]
    public int? ExpiresAt { get; set; }

    [JsonProperty("failed_at")]
    public int? FailedAt { get; set; }

    //[JsonProperty("last_error")]
    //public Error LastError { get; set; } // Assuming the Error class is defined elsewhere with JSON properties

    [JsonProperty("step_details")]
    public StepDetails StepDetails { get; set; } // Assuming a separate StepDetails class
}

public class StepDetails
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("message_creation")]
    public MessageCreationDetails MessageCreation { get; set; }
}

public class MessageCreationDetails
{
    [JsonProperty("message_id")]
    public string MessageId { get; set; }
}
