// Email Service
// ========================================

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

        public async Task SendVerificationCodeEmail(string email, string code, string name)
        {
            var html = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                <title>Verification Code</title>
                <style>
                    body {{
                        margin: 0;
                        padding: 0;
                        background-color: #e1e9d8;
                        font-family: Arial, sans-serif;
                        color: #000000;
                    }}

                    .header {{
                        background-color: #9fb881;
                        padding: 20px;
                        text-align: left;
                    }}

                    .logo {{
                        font-size: 24px;
                        color: #ffffff;
                        font-weight: bold;
                    }}

                    .content {{
                        padding: 40px 20px;
                        text-align: center;
                    }}

                    .title {{
                        font-size: 24px;
                        font-weight: normal;
                        margin-bottom: 10px;
                    }}

                    .highlight {{
                        font-weight: bold;
                        color: #4a6a3d;
                    }}

                    .message {{
                        margin-top: 10px;
                        font-size: 16px;
                        color: #000000;
                    }}

                    .icon {{
                        margin: 40px auto;
                    }}

                    .code-box {{
                        display: inline-block;
                        margin-top: 20px;
                        background-color: #9fb881;
                        padding: 14px 40px;
                        border-radius: 30px;
                        font-size: 24px;
                        font-weight: bold;
                        color: white;
                        letter-spacing: 2px;
                    }}

                    .footer {{
                        margin-top: 60px;
                        font-weight: bold;
                        color: #9fb881;
                    }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <div class='logo'>Coriander HR</div>
                </div>
                <div class='content'>
                    <div class='title'>Hey <span class='highlight'>{name}!</span></div>
                    <div class='message'>
                        Thank you for using <span class='highlight'>Coriander</span>
                    </div>

                    <div class='icon'>
                        <svg width='57' height='57' viewBox='0 0 57 57' fill='none' xmlns='http://www.w3.org/2000/svg'>
                            <path d='M16.625 42.75C12.6667 42.75 9.30208 41.3646 6.53125 38.5937C3.76042 35.8229 2.375 32.4583 2.375 28.5C2.375 24.5417 3.76042 21.1771 6.53125 18.4062C9.30208 15.6354 12.6667 14.25 16.625 14.25C19.2375 14.25 21.6323 14.9031 23.8094 16.2094C25.9865 17.5156 27.7083 19.2375 28.975 21.375H54.625V35.625H49.875V42.75H35.625V35.625H28.975C27.7083 37.7625 25.9865 39.4844 23.8094 40.7906C21.6323 42.0969 19.2375 42.75 16.625 42.75ZM16.625 38C19.2375 38 21.3354 37.1984 22.9187 35.5953C24.5021 33.9922 25.4521 32.4187 25.7687 30.875H40.375V38H45.125V30.875H49.875V26.125H25.7687C25.4521 24.5812 24.5021 23.0078 22.9187 21.4047C21.3354 19.8016 19.2375 19 16.625 19C14.0125 19 11.776 19.9302 9.91562 21.7906C8.05521 23.651 7.125 25.8875 7.125 28.5C7.125 31.1125 8.05521 33.349 9.91562 35.2094C11.776 37.0698 14.0125 38 16.625 38ZM16.625 33.25C17.9312 33.25 19.0495 32.7849 19.9797 31.8547C20.9099 30.9245 21.375 29.8062 21.375 28.5C21.375 27.1937 20.9099 26.0755 19.9797 25.1453C19.0495 24.2151 17.9312 23.75 16.625 23.75C15.3187 23.75 14.2005 24.2151 13.2703 25.1453C12.3401 26.0755 11.875 27.1937 11.875 28.5C11.875 29.8062 12.3401 30.9245 13.2703 31.8547C14.2005 32.7849 15.3187 33.25 16.625 33.25Z' fill='#6D8650'/>
                        </svg>
                    </div>

                    <div class='message'>Your <span class='highlight'>verification code</span> is</div>
                    <div class='code-box'>{code}</div>

                    <div class='footer'>
                        Best regards<br />
                        The Coriander Team
                    </div>
                </div>
            </body>
            </html>";

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verify your Coriander account";
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("SMTP_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendAccountPendingEmail(string email, string name)
        {
            var html = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                <title>Account Pending</title>
                <style>
                    body {{
                        margin: 0;
                        padding: 0;
                        background-color: #e1e9d8;
                        font-family: Arial, sans-serif;
                        color: #000000;
                    }}
                    .header {{
                        background-color: #9fb881;
                        padding: 20px;
                        text-align: left;
                    }}
                    .logo {{
                        font-size: 24px;
                        color: #ffffff;
                        font-weight: bold;
                    }}
                    .content {{
                        padding: 40px 20px;
                        text-align: center;
                    }}
                    .title {{
                        font-size: 24px;
                        font-weight: normal;
                        margin-bottom: 10px;
                    }}
                    .highlight {{
                        font-weight: bold;
                        color: #4a6a3d;
                    }}
                    .message {{
                        margin-top: 10px;
                        font-size: 16px;
                        color: #000000;
                    }}
                    .icon {{
                        margin: 40px auto;
                    }}
                    .subheading {{
                        font-size: 20px;
                        font-weight: bold;
                        color: #4a6a3d;
                        margin-bottom: 10px;
                    }}
                    .footer {{
                        margin-top: 60px;
                        font-weight: bold;
                        color: #9fb881;
                    }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <div class='logo'>Coriander</div>
                </div>
                <div class='content'>
                    <div class='title'>
                        Hey <span class='highlight'>{name}!</span><br />
                        Your account is <span class='highlight'>almost ready</span>
                    </div>
                    <div class='icon'>
                        <svg width='57' height='57' viewBox='0 0 57 57' fill='none' xmlns='http://www.w3.org/2000/svg'>
                            <path d='M20.425 53.4375L15.9125 45.8375L7.3625 43.9375L8.19375 35.15L2.375 28.5L8.19375 21.85L7.3625 13.0625L15.9125 11.1625L20.425 3.5625L28.5 7.00625L36.575 3.5625L41.0875 11.1625L49.6375 13.0625L48.8062 21.85L54.625 28.5L48.8062 35.15L49.6375 43.9375L41.0875 45.8375L36.575 53.4375L28.5 49.9937L20.425 53.4375ZM22.4437 47.3812L28.5 44.7687L34.675 47.3812L38 41.6812L44.5312 40.1375L43.9375 33.4875L48.3312 28.5L43.9375 23.3937L44.5312 16.7437L38 15.3187L34.5562 9.61875L28.5 12.2312L22.325 9.61875L19 15.3187L12.4687 16.7437L13.0625 23.3937L8.66875 28.5L13.0625 33.4875L12.4687 40.2562L19 41.6812L22.4437 47.3812ZM26.0062 36.9312L39.425 23.5125L36.1 20.0687L26.0062 30.1625L20.9 25.175L17.575 28.5L26.0062 36.9312Z' fill='#52643C'/>
                        </svg>
                    </div>
                    <div class='subheading'>Hooray!</div>
                    <div class='message'>
                        Your account is still pending and an admin still has to<br />
                        verify your account
                    </div>
                    <div class='footer'>
                        Best regards<br />
                        The Coriander Team
                    </div>
                </div>
            </body>
            </html>";

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your Coriander account is almost ready!";
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("SMTP_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendAccountActivatedEmailAsync(string email, string name, List<string> equipmentTitles)
        {
            string equipmentSection = "";

            if (equipmentTitles.Any())
            {
                string equipmentItems = string.Join("\n", equipmentTitles.Select(title =>
                    $"<div class='equipment-item'>{System.Net.WebUtility.HtmlEncode(title)}</div>"));

                equipmentSection = $@"
                <div class='equipment-section'>
                    <div class='equipment-title'>
                        You have been assigned with the following<br />
                        equipment items
                    </div>
                    {equipmentItems}
                </div>";
            }

            var html = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                <title>Account Ready</title>
                <style>
                    body {{ background-color: #e1e9d8; font-family: Arial, sans-serif; color: #000; margin: 0; padding: 0; }}
                    .header {{ background-color: #9fb881; padding: 20px; text-align: left; }}
                    .logo {{ font-size: 24px; color: #fff; font-weight: bold; }}
                    .content {{ padding: 40px 20px; text-align: center; }}
                    .title {{ font-size: 24px; margin-bottom: 10px; }}
                    .highlight {{ font-weight: bold; color: #4a6a3d; }}
                    .message {{ margin-top: 10px; font-size: 16px; }}
                    .icon {{ margin: 40px auto 20px auto; }}
                    .equipment-section {{
                        background-color: #fff; margin: 40px auto; padding: 30px 20px;
                        border-radius: 12px; max-width: 500px; text-align: center;
                    }}
                    .equipment-title {{ font-size: 16px; margin-bottom: 20px; }}
                    .equipment-item {{
                        background-color: #9fb881; color: #fff; padding: 12px 20px;
                        margin: 8px auto; border-radius: 999px; width: 80%;
                        max-width: 300px; font-weight: bold;
                    }}
                    .footer {{ margin-top: 60px; font-weight: bold; color: #9fb881; }}
                </style>
            </head>
            <body>
                <div class='header'><div class='logo'>Coriander</div></div>
                <div class='content'>
                    <div class='title'>
                        Hey <span class='highlight'>{System.Net.WebUtility.HtmlEncode(name)}!</span><br />
                        Your account is <span class='highlight'>ready!</span>
                    </div>
                    <div class='icon'>
                        <!-- Add an SVG if you want -->
                    </div>
                    <div class='message'>
                        The management team has successfully linked your<br />
                        account and you can sign in now
                    </div>
                    {equipmentSection}
                    <div class='footer'>
                        Best regards<br />The Coriander Team
                    </div>
                </div>
            </body>
            </html>";

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your Coriander account is ready!";
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("SMTP_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
