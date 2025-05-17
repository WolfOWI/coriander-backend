using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Services;

public class MeetingService : IMeetingService
{

    private readonly AppDbContext _context;

    public MeetingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MeetingRequestCreateDTO> CreateMeetingRequest(MeetingRequestCreateDTO meeting)
    {
        var meetingRequest = new Meeting
        {
            AdminId = meeting.AdminId,
            EmployeeId = meeting.EmployeeId,
            Purpose = meeting.Purpose,
            RequestedAt = DateTime.UtcNow,
            Status = MeetStatus.Requested
        };

        _context.Meetings.Add(meetingRequest);
        await _context.SaveChangesAsync();

        return meeting;
    }
}
