using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DowiezPlBackend.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }
        
        public bool Banned { get; set; }
    }
}