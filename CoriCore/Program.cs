using System.Diagnostics;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using CoriCore.Interfaces;
using CoriCore.Services; // Library for loading environment variables from a .env file
using CoriCore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


// Load environment variables (from .env file)
Env.Load();

// Debug logging for Google Meet environment variables
Console.WriteLine("Debug: Loading Google Meet environment variables");
Console.WriteLine($"GMEET_CLIENT_ID: {Environment.GetEnvironmentVariable("GMEET_CLIENT_ID")}");
Console.WriteLine($"GMEET_SCOPE: {Environment.GetEnvironmentVariable("GMEET_SCOPE")}");
Console.WriteLine($"GMEET_REDIRECT_URL: {Environment.GetEnvironmentVariable("GMEET_REDIRECT_URL")}");

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
// EmpLeaveRequest Service
builder.Services.AddScoped<IEmpLeaveRequestService, EmpLeaveRequestService>();
// Leave Request Service
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
// Apply For Leave Service
builder.Services.AddScoped<IApplyForLeaveService, ApplyForLeaveService>();
// Performance Review Service
builder.Services.AddScoped<IPerformanceReviewService, PerformanceReviewService>();
// Meeting Service
builder.Services.AddScoped<IMeetingService, MeetingService>();
// Gathering Service
builder.Services.AddScoped<IGatheringService, GatheringService>();
// Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
// Equipment Service
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
// Page Service (For whole front-end pages)
builder.Services.AddScoped<IPageService, PageService>();
// Image service - Local store
builder.Services.AddScoped<IImageService, ImageService>();
// Google Meet Token Service
builder.Services.AddScoped<IGMeetTokenService, GMeetTokenService>();
// Google Meet Service
builder.Services.AddScoped<IGoogleMeetService, GoogleMeetService>();
// ========================================

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Ek het port 5121 bygesit omdat myne op daai een run - Ruan
        policy.WithOrigins(
            "https://coriander-backend.onrender.com",
            "http://localhost:5173",   // your React dev server
            "https://localhost:5121",  // Swagger UI runs here
            "http://localhost:5121",    // fallback
            "localhost:5121"    // fallback
        )
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

// Google Authentication, Cookies and JWT management
// ========================================
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new Exception("JWT_SECRET environment variable is not set");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie()
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var headerToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(headerToken))
            {
                context.Token = headerToken;
            }
            else
            {
                var cookieToken = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
            }

            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
// ========================================

builder.Services.AddOpenApi(); // Add OpenAPI support
builder.Services.AddEndpointsApiExplorer(); // Add endpoints explorer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoriCore", Version = "v1" });
    c.OperationFilter<SwaggerFileUploadOperationFilter>();

    // Add JWT Authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


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

// Configure Google Meet settings from environment variables
builder.Services.Configure<GoogleMeetOptions>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("GMEET_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("GMEET_CLIENT_SECRET");
    options.Scope = Environment.GetEnvironmentVariable("GMEET_SCOPE");
    options.RedirectUrl = Environment.GetEnvironmentVariable("GMEET_REDIRECT_URL");
});

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

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware for Google Authentication
app.UseAuthentication(); // ✅ Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

// Fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
