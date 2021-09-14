using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Dtos.Opinion
{
    public class OpinionReadDto
    {
        public Guid OpinionId { get; set; }
        public DateTime CreationDate { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public AccountLimitedReadDto Issuer { get; set; }
        public AccountLimitedReadDto Rated { get; set; }
    }
}