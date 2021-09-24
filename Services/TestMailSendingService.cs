using System.IO;
using System.Threading.Tasks;
using DowiezPlBackend.Data;
using DowiezPlBackend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DowiezPlBackend.Services
{
    public class TestMailSendingService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public TestMailSendingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port,MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailConfirmationAsync(string emailAddress, string userId, string token)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(emailAddress));
            email.Subject = "Potwierdzenie adresu Email";

            string filePath = Directory.GetCurrentDirectory() + "/Templates/EmailConfirmationTemplate.html";
            StreamReader str = new StreamReader(filePath);
            string text = await str.ReadToEndAsync();
            str.Close();

            var builder = new BodyBuilder();
            builder.HtmlBody = text.Replace("[USERID]", userId).Replace("[TOKEN]", token);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port,MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}