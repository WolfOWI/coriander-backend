// Email Service Interface
// ========================================

using System;

namespace CoriCore.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string messageTitle, string messageBody);
        Task SendVerificationCodeEmail(string email, string code, string name);
        Task SendAccountPendingEmail(string email, string name);
        Task SendAccountActivatedEmailAsync(string email, string name, List<string> equipmentTitles);
    }
}
