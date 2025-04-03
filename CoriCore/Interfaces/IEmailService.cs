using System;

namespace CoriCore.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string messageTitle, string messageBody);
    }
}
