using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Settings;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ArtemisBanking.Infraestructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(MailSettings mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                emailRequestDto.ToRange?.Add(emailRequestDto.To ?? "");
                MimeMessage email = new()
                {
                    Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
                    Subject = emailRequestDto.Subject
                };

                foreach (var item in emailRequestDto.ToRange ?? [])
                {
                    email.To.Add(MailboxAddress.Parse(item));
                }

                BodyBuilder builder = new()
                {
                    HtmlBody = emailRequestDto.HtmlBody
                };

                email.Body = builder.ToMessageBody();

                using MailKit.Net.Smtp.SmtpClient smtpClient = new();
                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured {Exception}.", ex);
            }
        }
    }
}
