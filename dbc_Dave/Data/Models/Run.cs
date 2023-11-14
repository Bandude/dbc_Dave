using System;
using System.Collections.Generic;
using dbc_Dave.Data.Models;
public class Run
{
    public string Id { get; set; }
    public string Object { get; set; }
    public int CreatedAt { get; set; }
    public string ThreadId { get; set; }
    public string AssistantId { get; set; }
    public string Status { get; set; }
    public RequiredAction RequiredAction { get; set; } // Details not provided, assuming separate class
    public Error LastError { get; set; } // Details not provided, assuming separate class
    public int ExpiresAt { get; set; }
    public int? StartedAt { get; set; }
    public int? CancelledAt { get; set; }
    public int? FailedAt { get; set; }
    public int? CompletedAt { get; set; }
    public string Model { get; set; }
    public string Instructions { get; set; }
    public List<Tool> Tools { get; set; } // Assuming Tool is a separate class
    public List<string> FileIds { get; set; }
    public Dictionary<string, string> Metadata { get; set; } // Assuming maximum size enforced elsewhere
    // ... Constructor, methods, etc.
}
public class RequiredAction
{
    // Properties as per your "Show properties" instruction
    // ...
}
public class Error
{
    // Properties as per your "Show properties" instruction
    // ...
}
