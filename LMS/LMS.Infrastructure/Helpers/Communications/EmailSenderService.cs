﻿using LMS.Application.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using LMS.Application.Abstractions.Communications;

namespace LMS.Infrastructure.Helpers.Communications
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var fromEmail = _emailSettings.FromEmail;
            var password = _emailSettings.Password;
            var host = _emailSettings.Host;
            var port = _emailSettings.Port;
            var logoUrl = _emailSettings.LogoUrl;

            message = message.Replace("{{logoUrl}}", logoUrl);

            var mail = new MimeMessage();
            mail.From.Add(MailboxAddress.Parse(fromEmail));
            mail.To.Add(MailboxAddress.Parse(toEmail));

            mail.Subject = subject;
            mail.Body = new TextPart(TextFormat.Html) { Text = message };

            var smtp = new SmtpClient();

            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(fromEmail, password);

            await smtp.SendAsync(mail);

            await smtp.DisconnectAsync(true);
        }
    }
}
