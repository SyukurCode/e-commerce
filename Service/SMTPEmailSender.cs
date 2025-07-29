
using System.Net.Mail;
using System.Net;
using E_Commers_Adelia.Common;

namespace E_Commers_Adelia.Service
{
    public class SMTPEmailSender : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = EnvHelper.GetEnv("SMTP_HOST");
            var smtpPort = EnvHelper.GetEnv("SMTP_PORT");
            var smtpUser = EnvHelper.GetEnv("SMTP_USER");
            var smtpPass = EnvHelper.GetEnv("SMTP_PASS");
            var smtpFrom = EnvHelper.GetEnv("SMTP_FROM");

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPort) ||
                string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) ||
                string.IsNullOrEmpty(smtpFrom))
            {
                throw new InvalidOperationException("IMailSenser: One or more required environment variables are missing.");
            }

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = int.Parse(smtpPort),
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
