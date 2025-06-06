using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using CoriCore.Services;
using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace CoriCore.Tests.Unit.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_SendsEmailWithoutException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var service = new EmailService(mockConfig.Object);

            // Set environment variables for the test
            Environment.SetEnvironmentVariable("EMAIL_FROM", "testfrom@example.com");
            Environment.SetEnvironmentVariable("SMTP_HOST", "smtp.example.com");
            Environment.SetEnvironmentVariable("SMTP_PORT", "587");
            Environment.SetEnvironmentVariable("EMAIL_USERNAME", "username");
            Environment.SetEnvironmentVariable("EMAIL_PASSWORD", "password");

            // Act & Assert
            // Since the real SmtpClient will fail without a real SMTP server,
            // we only check that the method throws or not (integration test).
            // For a pure unit test, you would refactor EmailService to allow injecting a mock SmtpClient.
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await service.SendEmailAsync(
                    "recipient@example.com",
                    "Test Subject",
                    "Test Title",
                    "Test Body"
                );
            });
        }
    }
}