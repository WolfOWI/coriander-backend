using System.Diagnostics;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv; // Library for loading environment variables from a .env file

// Load environment variables (from .env file)
Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Add controllers
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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
