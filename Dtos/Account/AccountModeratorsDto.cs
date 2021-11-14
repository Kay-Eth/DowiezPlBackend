using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountModeratorsDto
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool Banned { get; set; }
    }
}