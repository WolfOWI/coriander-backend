namespace CoriCore.DTOs;

public class GoogleDateTimeDTO
{
    public DateTime DateTime { get; set; }
    public string TimeZone { get; set; }
}

public class GoogleAttendeeDTO
{
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string? ResponseStatus { get; set; } = "needsAction";
}

public class ConferenceDataDTO
{
    public ConferenceCreateRequestDTO CreateRequest { get; set; }
    public ConferenceStatusDTO Status { get; set; }
}

public class ConferenceCreateRequestDTO
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public ConferenceSolutionKeyDTO ConferenceSolutionKey { get; set; }
}

public class ConferenceSolutionKeyDTO
{
    public string Type { get; set; } = "hangoutsMeet";
}

public class ConferenceStatusDTO
{
    public string StatusCode { get; set; } = "success";
}