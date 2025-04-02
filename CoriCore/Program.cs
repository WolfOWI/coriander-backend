using System.Diagnostics;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using CoriCore.Interfaces;
using CoriCore.Services; // Library for loading environment variables from a .env file
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


// Load environment variables (from .env file)
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// SERVICES
// ========================================
// Authentication Service
builder.Services.AddScoped<IAuthService, AuthServices>();
// Employee Service
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Admin Service
builder.Services.AddScoped<IAdminService, AdminService>();
// User service
builder.Services.AddScoped<IUserService, UserService>();
// Leave Balance Service
builder.Services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
// EmpUser Service
builder.Services.AddScoped<IEmpUserService, EmpUserService>();
// Leave Request Service
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
// Performance Review Service
builder.Services.AddScoped<IPerformanceReviewRepository, PerformanceReviewRepository>();
// ========================================

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// CONTROLLERS
// ========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });// Prevent object loops (e.g: employee -> equipment -> employee ...)
// ========================================

// Google Authentication and Cookies
// ========================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
});
// ========================================

builder.Services.AddOpenApi(); // Add OpenAPI support
builder.Services.AddEndpointsApiExplorer(); // Add endpoints explorer
builder.Services.AddSwaggerGen(); // Adding Swagger Package

// Database connection configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to environment variables if connection string is not in config
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");
    var database = Environment.GetEnvironmentVariable("PGDATABASE");

    connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database};SSLMode=Require";
}

// Add the db context to the services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; // This makes Swagger UI the root page
    });

    // Section added to open browser automatically when "dotnet run" is executed
    // ------------------------------------------------------------------------
    var url = "http://localhost:5121";
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch
    {
        // If Process.Start fails, try platform specific commands
        if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", url);
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("xdg-open", url);
        }
    }
}
// ------------------------------------------------------------------------

app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

// Middleware for Google Authentication
app.UseAuthentication();

app.MapControllers();

app.Run();
