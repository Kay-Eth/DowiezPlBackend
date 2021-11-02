using System;
using System.IO;
using System.Threading.Tasks;
using DowiezPlBackend.Data;
using DowiezPlBackend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DowiezPlBackend.Services
{
    public class CbaPlMailSendingService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public CbaPlMailSendingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MimeMessage message)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port,MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailConfirmationAsync(string emailAddress, string userId, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(emailAddress));
            message.Subject = "Potwierdzenie adresu Email";

            string filePath = Directory.GetCurrentDirectory() + "/Templates/EmailConfirmationTemplate.html";
            StreamReader str = new StreamReader(filePath);
            string text = await str.ReadToEndAsync();
            str.Close();

            var builder = new BodyBuilder();
            builder.HtmlBody = text.Replace("[USERID]", userId).Replace("[TOKEN]", token);
            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendPasswordResetAsync(string emailAddress, string userId, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(emailAddress));
            message.Subject = "Reset hasła";

            string filePath = Directory.GetCurrentDirectory() + "/Templates/PasswordResetTemplate.html";
            StreamReader str = new StreamReader(filePath);
            string text = await str.ReadToEndAsync();
            str.Close();

            var builder = new BodyBuilder();
            // var token_alter = token.Replace("/", "%2F").Replace("+", "%2B");
            // Console.WriteLine(token_alter);
            builder.HtmlBody = text.Replace("[USERID]", userId).Replace("[TOKEN]", token);
            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendBannedAsync(string emailAddress, string moderatorEmailAddress, string moderatorName, string moderatorId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(emailAddress));
            message.Subject = "Twoje konto zostało zbanowane";

            string filePath = Directory.GetCurrentDirectory() + "/Templates/BannedTemplate.html";
            StreamReader str = new StreamReader(filePath);
            string text = await str.ReadToEndAsync();
            str.Close();

            var builder = new BodyBuilder();
            builder.HtmlBody = text
                .Replace("[NAME]", moderatorName)
                .Replace("[EMAIL]", moderatorEmailAddress)
                .Replace("[ID]", moderatorId);
            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendModeratorPasswordResetAsync(string emailAddress, string userId, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(emailAddress));
            message.Subject = "Reset hasła";

            string filePath = Directory.GetCurrentDirectory() + "/Templates/ModeratorPasswordResetTemplate.html";
            StreamReader str = new StreamReader(filePath);
            string text = await str.ReadToEndAsync();
            str.Close();

            var token_alter = token.Replace("/", "%2F").Replace("+", "%2B");
            var builder = new BodyBuilder();
            builder.HtmlBody = text.Replace("[USERID]", userId).Replace("[TOKEN]", token_alter);
            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }
    }
}