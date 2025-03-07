using System;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Data;

public class AppDbContext : DbContext
{

    // Constructor - use all the base context options for the db context
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    // Tables in the database
    // ------------------------------------------------------------------------
    // User table
    public DbSet<User> Users { get; set; }

    // Admin table
    public DbSet<Admin> Admins { get; set; }

    // ------------------------------------------------------------------------
}
