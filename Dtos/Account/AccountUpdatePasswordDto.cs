using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountUpdatePasswordDto
    {
        [MaxLength(255)]
        public string OldPassword { get; set; }

        [MaxLength(255)]
        public string NewPassword { get; set; }
    }
}