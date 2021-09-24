using System.Threading.Tasks;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendEmailConfirmationAsync(string emailAddress, string userId, string token);
    }
}