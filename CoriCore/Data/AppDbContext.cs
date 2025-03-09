using System;
using CoriCore.Migrations;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Data;

public class AppDbContext : DbContext
{

    // Constructor - use all the base context options for the db context
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tables in the database
    // ------------------------------------------------------------------------
    // User table
    public DbSet<User> Users { get; set; }

    // Admin table
    public DbSet<Admin> Admins { get; set; }

    // Employee table
    public DbSet<Employee> Employees { get; set; }

    // PayCycle table
    public DbSet<PayCycle> PayCycles { get; set; }

    // Equipment table
    public DbSet<Equipment> Equipments { get; set; }

    // EquipmentCategory table
    public DbSet<EquipmentCategory> EquipmentCategories { get; set; }

    // LeaveBalance table
    public DbSet<LeaveBalance> LeaveBalances { get; set; }

    // LeaveRequest table
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    // LeaveType table
    public DbSet<LeaveType> LeaveTypes { get; set; }


    // ------------------------------------------------------------------------
}
