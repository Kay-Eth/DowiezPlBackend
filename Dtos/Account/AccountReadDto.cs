using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountReadDto
    {
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
    }
}