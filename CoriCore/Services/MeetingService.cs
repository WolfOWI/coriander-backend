using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class MeetingService : IMeetingService
{

    private readonly AppDbContext _context;

    public MeetingService(AppDbContext context)
    {
        _context = context;
    }

    // GET
    // ========================================
    // EMPLOYEE RELATED 
    // ------------------------
    public async Task<IEnumerable<MeetingDTO>> GetAllMeetingsByEmployeeId(int employeeId)
    {
        var meetings = await _context.Meetings
            .Where(m => m.EmployeeId == employeeId)
            .Include(m => m.Admin)
                .ThenInclude(a => a.User)
            .Include(m => m.Employee)
                .ThenInclude(e => e.User)
            .ToListAsync();
        return meetings.Select(m => new MeetingDTO
        {
            MeetingId = m.MeetingId,
            AdminId = m.AdminId,
            AdminName = m.Admin.User.FullName,
            EmployeeId = m.EmployeeId,
            EmployeeName = m.Employee.User.FullName,
            IsOnline = m.IsOnline,
            MeetLocation = m.MeetLocation,
            MeetLink = m.MeetLink,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Purpose = m.Purpose,
            RequestedAt = m.RequestedAt,
            Status = m.Status,
        });
    }

    public async Task<IEnumerable<MeetingDTO>> GetMeetingsByEmployeeIdAndStatus(int employeeId, MeetStatus status)
    {
        var meetings = await _context.Meetings
            .Where(m => m.EmployeeId == employeeId)
            .Include(m => m.Admin)
                .ThenInclude(a => a.User)
            .Include(m => m.Employee)
                .ThenInclude(e => e.User)
            .Where(m => m.Status == status) // Filter by status
            .ToListAsync();

        return meetings.Select(m => new MeetingDTO
        {
            MeetingId = m.MeetingId,
            AdminId = m.AdminId,
            AdminName = m.Admin.User.FullName,
            EmployeeId = m.EmployeeId,
            EmployeeName = m.Employee.User.FullName,
            IsOnline = m.IsOnline,
            MeetLocation = m.MeetLocation,
            MeetLink = m.MeetLink,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Purpose = m.Purpose,
            RequestedAt = m.RequestedAt,
            Status = m.Status,
        });
    }
    // ------------------------

    // ADMIN RELATED 
    // ------------------------
    public async Task<IEnumerable<MeetingDTO>> GetAllMeetingsByAdminId(int adminId)
    {
        var meetings = await _context.Meetings
            .Where(m => m.AdminId == adminId)
            .Include(m => m.Admin)
                .ThenInclude(a => a.User)
            .Include(m => m.Employee)
                .ThenInclude(e => e.User)
            .ToListAsync();
        return meetings.Select(m => new MeetingDTO
        {
            MeetingId = m.MeetingId,
            AdminId = m.AdminId,
            AdminName = m.Admin.User.FullName,
            EmployeeId = m.EmployeeId,
            EmployeeName = m.Employee.User.FullName,
            IsOnline = m.IsOnline,
            MeetLocation = m.MeetLocation,
            MeetLink = m.MeetLink,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Purpose = m.Purpose,
            RequestedAt = m.RequestedAt,
            Status = m.Status,
        });
    }
    // ------------------------
    // ========================================

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

    public async Task<(int Code, string Message)> RejectMeetingRequest(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        meeting.Status = MeetStatus.Rejected; // update status to rejected

        try
        {
            await _context.SaveChangesAsync();
            return (200, "Meeting request rejected successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error rejecting meeting request: {ex.Message}");
        }
    }

    public async Task<(int Code, string Message)> MarkMeetingAsCompleted(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        meeting.Status = MeetStatus.Completed; // update status to completed

        try
        {
            await _context.SaveChangesAsync();
            return (200, "Meeting marked as completed successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error marking meeting as completed: {ex.Message}");
        }
    }
    // ========================================
}
