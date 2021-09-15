using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountUpdatePasswordDto
    {
        [Required]
        [MaxLength(255)]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(255)]
        public string NewPassword { get; set; }
    }
}