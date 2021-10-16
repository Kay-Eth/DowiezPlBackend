using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountResetPasswordDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; }
        
        [Required]
        [MinLength(8)]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}