using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}