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

    public async Task<IEnumerable<MeetingDTO>> GetMeetingsByAdminIdAndStatus(int adminId, MeetStatus status)
    {
        var meetings = await _context.Meetings
            .Where(m => m.AdminId == adminId)
            .Where(m => m.Status == status)
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

    public async Task<IEnumerable<MeetingRequestDTO>> GetAllPendingRequestsByAdminId(int adminId)
    {
        var meetings = await _context.Meetings
            .Where(m => m.AdminId == adminId)
            .Where(m => m.Status == MeetStatus.Requested)
            .Include(m => m.Employee)
                .ThenInclude(e => e.User)
            .OrderByDescending(m => m.RequestedAt)
            .ToListAsync();

        return meetings.Select(m => new MeetingRequestDTO
        {
            MeetingId = m.MeetingId,
            EmployeeId = m.EmployeeId,
            EmployeeName = m.Employee.User.FullName,
            ProfilePicture = m.Employee.User.ProfilePicture,
            Purpose = m.Purpose,
            RequestedAt = m.RequestedAt,
            Status = m.Status,
        });
    }
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
    public async Task<(int Code, string Message)> ConfirmAndUpdateMeetingRequest(int meetingId, MeetingUpdateDTO dto)
    {
        // Check if meeting exists
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        // Update meeting details
        meeting.IsOnline = dto.IsOnline;
        if (dto.MeetLocation != null) meeting.MeetLocation = dto.MeetLocation; // if location is provided, update it
        if (dto.MeetLink != null) meeting.MeetLink = dto.MeetLink; // if link is provided, update it
        if (dto.StartDate != null) meeting.StartDate = dto.StartDate;
        if (dto.EndDate != null) meeting.EndDate = dto.EndDate;
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

    public async Task<(int Code, string Message)> UpdateMeeting(int meetingId, MeetingUpdateDTO dto)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        // Update meeting details
        meeting.IsOnline = dto.IsOnline;
        meeting.MeetLocation = dto.MeetLocation;
        meeting.MeetLink = dto.MeetLink;
        meeting.StartDate = dto.StartDate;
        meeting.EndDate = dto.EndDate;
        meeting.Status = dto.Status ?? meeting.Status;

        try
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
            return (200, "Meeting request updated successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error updating meeting request details: {ex.Message}");
        }
    }

    public async Task<(int Code, string Message)> UpdateMeetingRequest(int meetingId, MeetingRequestUpdateDTO dto)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        // Check if the meetingId has requested status
        if (meeting.Status != MeetStatus.Requested)
        {
            return (400, "Meeting request is not in requested status");
        }

        // Update meeting request details (only if provided)
        if (dto.AdminId.HasValue) meeting.AdminId = dto.AdminId.Value;
        if (dto.Purpose != null) meeting.Purpose = dto.Purpose;

        try
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
            return (200, "Meeting request updated successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error updating meeting request details: {ex.Message}");
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

    public async Task<(int Code, string Message)> MarkMeetingAsUpcoming(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        meeting.Status = MeetStatus.Upcoming; // update status to upcoming

        try
        {
            await _context.SaveChangesAsync();
            return (200, "Meeting marked as upcoming successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error marking meeting as upcoming: {ex.Message}");
        }
    }
    // ========================================

    // DELETE
    // ========================================
    public async Task<(int Code, string Message)> DeleteMeeting(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            return (404, "Meeting not found");
        }

        try
        {
            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();
            return (200, "Meeting deleted successfully");
        }
        catch (Exception ex)
        {
            return (500, $"Error deleting meeting: {ex.Message}");
        }
    }
    // ========================================
}
