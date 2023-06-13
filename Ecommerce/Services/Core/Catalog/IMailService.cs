using MailKit.Net.Smtp;
using MimeKit;

namespace Ecommerce.Services.Core.Catalog
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }

    public class GmailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public GmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var email = new MimeMessage();
            var from = new MailboxAddress(_configuration["MailSettings:DisplayName"], _configuration["MailSettings:Mail"]);
            email.From.Add(from);

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["MailSettings:Host"], int.Parse(_configuration["MailSettings:Port"]), true);
            await smtp.AuthenticateAsync(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}