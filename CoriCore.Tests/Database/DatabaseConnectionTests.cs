// Wolf Botha

using Xunit;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;  // For loading .env file

namespace CoriCore.Tests.Database
{
    /// <summary>
    /// Testing if our application can successfully connect to our PostgreSQL database
    /// </summary>
    public class DatabaseConnectionTests
    {
        // [Fact]  // Marks this method as a test that xUnit should run
        // public async Task CanConnectToDatabase()  // Async method because database operations are asynchronous
        // {
        //     // ARRANGE SECTION (Set up everything needed for our test)

        //     // Load .env from the test project root (instead of in the bin/Debug/net9.0 directory)
        //     Env.Load(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"));

        //     // Build connection string from environment variables
        //     var connectionString = $"Host={Environment.GetEnvironmentVariable("PGHOST")};" + $"Port={Environment.GetEnvironmentVariable("PGPORT")};" + $"Username={Environment.GetEnvironmentVariable("PGUSER")};" + $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")};" + $"Database={Environment.GetEnvironmentVariable("PGDATABASE")};" + "SSL Mode=Require";
            
        //     // Create options for our database context
        //     // DbContextOptionsBuilder helps configure our database connection
        //     var options = new DbContextOptionsBuilder<AppDbContext>()
        //         // UseNpgsql configures the context to use PostgreSQL
        //         .UseNpgsql(connectionString)
        //         .Options;  // Convert the builder to Options that we can use

        //     // Create a new instance of our database context using the options
        //     var context = new AppDbContext(options);

        //     // ACT & ASSERT SECTION (Perform the test and check the results)
            
        //     // CanConnectAsync() attempts to establish a connection to the database
        //     // It returns true if successful, false if it fails
        //     bool canConnect = await context.Database.CanConnectAsync();

        //     // Assert.True() is an xUnit assertion that verifies the condition is true
        //     // If canConnect is false, the test will fail with the message provided
        //     Assert.True(canConnect, "Database connection failed.");
            
        // }
    }
}
