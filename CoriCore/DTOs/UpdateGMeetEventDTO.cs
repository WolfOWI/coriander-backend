using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class UpdateGMeetEventDTO
{
    public GoogleDateTimeDTO? Start { get; set; }
    public GoogleDateTimeDTO? End { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public List<GoogleAttendeeDTO>? Attendees { get; set; }
    public ConferenceDataDTO? ConferenceData { get; set; }
}