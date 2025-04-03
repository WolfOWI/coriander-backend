using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using CoriCore.Interfaces;

namespace CoriCore.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string messageTitle, string messageBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = subject;

            string html = $@"<h2>{messageTitle}</h2><p>{messageBody}</p>";
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("SMTP_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
