using System;
using System.Collections.Generic;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class CreateGMeetEventDTO
{
    // Required fields
    public string Summary { get; set; } = string.Empty;
    public List<string> AttendeeEmails { get; set; } = new();
    
    // Optional fields with defaults
    public string? Description { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public int? DurationMinutes { get; set; } = 60; // Default 1 hour if no specific times provided
}

// Helper class for the service to use
public class GMeetEventRequest
{
    public EventDateTime Start { get; set; } = null!;
    public EventDateTime End { get; set; } = null!;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<EventAttendee> Attendees { get; set; } = new();
    public ConferenceData ConferenceData { get; set; } = null!;
}

public class EventDateTime
{
    public string DateTime { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "Africa/Johannesburg";
}

public class EventAttendee
{
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string ResponseStatus { get; set; } = "needsAction";
}

public class ConferenceData
{
    public CreateRequest CreateRequest { get; set; } = null!;
    public Status Status { get; set; } = new();
}

public class CreateRequest
{
    public string RequestId { get; set; } = string.Empty;
    public ConferenceSolutionKey ConferenceSolutionKey { get; set; } = new();
}

public class ConferenceSolutionKey
{
    public string Type { get; set; } = "hangoutsMeet";
}

public class Status
{
    public string StatusCode { get; set; } = "success";
}
