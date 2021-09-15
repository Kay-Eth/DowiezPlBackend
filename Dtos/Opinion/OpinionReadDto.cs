using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Dtos.Opinion
{
    public class OpinionReadDto
    {
        [Required]
        public Guid OpinionId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public int Rating { get; set; }
        public string Description { get; set; }
        [Required]
        public AccountLimitedReadDto Issuer { get; set; }
        [Required]
        public AccountLimitedReadDto Rated { get; set; }
    }
}