using System;

namespace DowiezPlBackend.Dtos.Account
{
    public class AccountTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}