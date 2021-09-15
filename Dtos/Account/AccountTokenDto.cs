using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountTokenDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime Expiration { get; set; }
    }
}