using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountUpdateDataDto
    {
        [MaxLength(200)]
        public string UserFirstName { get; set; }

        [MaxLength(200)]
        public string UserLastName { get; set; }
    }
}