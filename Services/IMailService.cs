using System.Threading.Tasks;
using DowiezPlBackend.Models;
using MimeKit;

namespace DowiezPlBackend.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MimeMessage mailRequest);
        Task SendEmailConfirmationAsync(string emailAddress, string userId, string token);
        Task SendPasswordResetAsync(string emailAddress, string userId, string token);
        Task SendModeratorPasswordResetAsync(string emailAddress, string userId, string token);
        Task SendBannedAsync(string emailAddress, string moderatorEmailAddress, string moderatorName, string moderatorId);
    }
}