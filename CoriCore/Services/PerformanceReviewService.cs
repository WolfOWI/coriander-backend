using System;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class PerformanceReviewService : IPerformanceReviewService
{
    // ========================================
    // DEPENDENCY INJECTION
    // ========================================
    private readonly AppDbContext _context;

    public PerformanceReviewService(AppDbContext context)
    {
        _context = context;
    }

    // ========================================
    // GET PERFORMANCE REVIEW BY START DATE AND ADMIN ID
    // ========================================
    public async Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate)
    {
        return await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User) // Get Admin's Name
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User) // Get Employee's Name
            .Where(pr => pr.AdminId == adminId && pr.StartDate.Date == startDate.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId)
    {
        return await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User) // Include Admin's Name
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User) // Include Employee's Name
            .Where(pr => pr.EmployeeId == employeeId)
            .ToListAsync();
    }



}