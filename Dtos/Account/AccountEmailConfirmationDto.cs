using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountEmailConfirmationDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}