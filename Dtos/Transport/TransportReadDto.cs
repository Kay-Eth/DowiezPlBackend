using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Transport
{
    public class TransportReadDto
    {
        [Required]
        public Guid TransportId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime TransportDate { get; set; }
        public string Description { get; set; }
        [Required]
        public TransportStatus Status { get; set; }

        [Required]
        public TransportCategory Category { get; set; }
        [Required]
        public CityReadDto StartsIn { get; set; }
        [Required]
        public CityReadDto EndsIn { get; set; }
        [Required]
        public AccountLimitedReadDto Creator { get; set; }
    }
}