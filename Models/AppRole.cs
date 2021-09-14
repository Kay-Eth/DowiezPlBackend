using System;
using Microsoft.AspNetCore.Identity;

namespace DowiezPlBackend.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public AppRole() : base()
        {
        }

        public AppRole(string roleName) : base(roleName)
        {
        }
    }
}