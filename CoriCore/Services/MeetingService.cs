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

    // CREATE
    // ========================================
    public async Task<MeetingRequestCreateDTO> CreateMeetingRequest(MeetingRequestCreateDTO meeting)
    {

        var admin = await _context.Admins.FindAsync(meeting.AdminId);
        if (admin == null)
        {
            throw new Exception("Admin not found");
        }

        var employee = await _context.Employees.FindAsync(meeting.EmployeeId);
        if (employee == null)
        {
            throw new Exception("Employee not found");
        }


        var meetingRequest = new Meeting
        {
            AdminId = meeting.AdminId,
            EmployeeId = meeting.EmployeeId,
            Purpose = meeting.Purpose,
            RequestedAt = DateTime.UtcNow,
            Status = MeetStatus.Requested,
        };

        _context.Meetings.Add(meetingRequest);
        await _context.SaveChangesAsync();

        return meeting;
    }
    // ========================================

    // UPDATE
    // ========================================
    public async Task<(int Code, string Message)> ConfirmAndUpdateMeetingRequest(int meetingId, MeetingConfirmDTO meetingConfirmDTO)
    {
        // Check if meeting exists
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        // Update meeting details
        meeting.IsOnline = meetingConfirmDTO.IsOnline;
        if (meetingConfirmDTO.MeetLocation != null) meeting.MeetLocation = meetingConfirmDTO.MeetLocation; // if location is provided, update it
        if (meetingConfirmDTO.MeetLink != null) meeting.MeetLink = meetingConfirmDTO.MeetLink; // if link is provided, update it
        meeting.StartDate = meetingConfirmDTO.StartDate;
        meeting.EndDate = meetingConfirmDTO.EndDate;
        meeting.Status = MeetStatus.Upcoming; // update status to upcoming

        try
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
            return (200, "Meeting request updated successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error updating meeting request: {ex.Message}");
        }
        
    }
    // ========================================
}
