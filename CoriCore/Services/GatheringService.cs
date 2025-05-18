using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class GatheringService : IGatheringService
{
    private readonly AppDbContext _context;
    private readonly IMeetingService _meetingService;
    private readonly IPerformanceReviewService _performanceReviewService;

    public GatheringService(
        AppDbContext context,
        IMeetingService meetingService,
        IPerformanceReviewService performanceReviewService)
    {
        _context = context;
        _meetingService = meetingService;
        _performanceReviewService = performanceReviewService;
    }

    // EMPLOYEE
    // ========================================
    public async Task<IEnumerable<GatheringDTO>> GetAllGatheringsByEmployeeId(int employeeId)
    {
        var gatherings = new List<GatheringDTO>();

        // Get all meetings for the employee (all statuses)
        foreach (MeetStatus status in Enum.GetValues(typeof(MeetStatus)))
        {
            var meetings = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, status);
            foreach (var meeting in meetings)
            {
                gatherings.Add(new GatheringDTO
                {
                    Id = meeting.MeetingId,
                    Type = GatheringType.Meeting,
                    AdminId = meeting.AdminId,
                    AdminName = meeting.AdminName ?? string.Empty,
                    EmployeeId = meeting.EmployeeId,
                    EmployeeName = meeting.EmployeeName ?? string.Empty,
                    IsOnline = meeting.IsOnline,
                    MeetLocation = meeting.MeetLocation,
                    MeetLink = meeting.MeetLink,
                    StartDate = meeting.StartDate,
                    EndDate = meeting.EndDate,
                    Purpose = meeting.Purpose,
                    RequestedAt = meeting.RequestedAt,
                    MeetingStatus = meeting.Status
                });
            }
        }

        // Get all performance reviews for the employee
        var reviews = await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User)
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User)
            .Where(pr => pr.EmployeeId == employeeId)
            .ToListAsync();

        foreach (var review in reviews)
        {
            gatherings.Add(new GatheringDTO
            {
                Id = review.ReviewId,
                Type = GatheringType.PerformanceReview,
                AdminId = review.AdminId,
                AdminName = review.Admin?.User?.FullName ?? string.Empty,
                EmployeeId = review.EmployeeId,
                EmployeeName = review.Employee?.User?.FullName ?? string.Empty,
                IsOnline = review.IsOnline,
                MeetLocation = review.MeetLocation,
                MeetLink = review.MeetLink,
                StartDate = review.StartDate,
                EndDate = review.EndDate,
                Rating = review.Rating,
                Comment = review.Comment,
                DocUrl = review.DocUrl,
                ReviewStatus = review.Status
            });
        }

        // Sort by start date
        return gatherings
            .OrderBy(g => g.StartDate)
            .ThenBy(g => g.Type); // Secondary sort by type for items with same start date
    }


    public async Task<IEnumerable<GatheringDTO>> GetAllGatheringsByEmployeeIdAndStatus(int employeeId, string status)
    {
        MeetStatus meetingStatus = MeetStatus.Upcoming;
        ReviewStatus reviewStatus = ReviewStatus.Upcoming;

        if (status == "Completed")
        {
            meetingStatus = MeetStatus.Completed;
            reviewStatus = ReviewStatus.Completed;
        }

        var gatherings = await GetAllGatheringsByEmployeeId(employeeId);
        return gatherings.Where(g => g.MeetingStatus == meetingStatus || g.ReviewStatus == reviewStatus);
    }
    // ========================================


    // ADMIN
    // ========================================
    public async Task<IEnumerable<GatheringDTO>> GetAllGatheringsByAdminId(int adminId)
    {
        var gatherings = new List<GatheringDTO>();

        // Get meetings
        var upcomingMeetings = await _meetingService.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming);
        foreach (var meeting in upcomingMeetings)
        {
            gatherings.Add(new GatheringDTO
            {
                Id = meeting.MeetingId,
                Type = GatheringType.Meeting,
                AdminId = meeting.AdminId,
                AdminName = meeting.AdminName ?? string.Empty,
                EmployeeId = meeting.EmployeeId,
                EmployeeName = meeting.EmployeeName ?? string.Empty,
                IsOnline = meeting.IsOnline,
                MeetLocation = meeting.MeetLocation,
                MeetLink = meeting.MeetLink,
                StartDate = meeting.StartDate,
                EndDate = meeting.EndDate,
                Purpose = meeting.Purpose,
                RequestedAt = meeting.RequestedAt,
                MeetingStatus = meeting.Status
            });
        }

        // Get performance reviews
        var reviews = await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User)
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User)
            .Where(pr => pr.AdminId == adminId)
            .ToListAsync();

        foreach (var review in reviews)
        {
            gatherings.Add(new GatheringDTO
            {
                Id = review.ReviewId,
                Type = GatheringType.PerformanceReview,
                AdminId = review.AdminId,
                AdminName = review.Admin?.User?.FullName ?? string.Empty,
                EmployeeId = review.EmployeeId,
                EmployeeName = review.Employee?.User?.FullName ?? string.Empty,
                IsOnline = review.IsOnline,
                MeetLocation = review.MeetLocation,
                MeetLink = review.MeetLink,
                StartDate = review.StartDate,
                EndDate = review.EndDate,
                Rating = review.Rating,
                Comment = review.Comment,
                DocUrl = review.DocUrl,
                ReviewStatus = review.Status
            });
        }

        return gatherings.OrderBy(g => g.StartDate);
    }
    // ========================================

}
