using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountUpdateDataDto
    {
        [MaxLength(200)]
        public string FirstName { get; set; }

        [MaxLength(200)]
        public string LastName { get; set; }
    }
}