using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Dtos.Group;
using DowiezPlBackend.Dtos.Transport;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Demand
{
    public class DemandReadDto
    {
        [Required]
        public Guid DemandId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        [Required]
        public DemandStatus Status { get; set; }
        [Required]
        public DemandCategory Category { get; set; }
        public CityReadDto From { get; set; }
        [Required]
        public CityReadDto Destination { get; set; }
        [Required]
        public AccountLimitedReadDto Creator { get; set; }
        public AccountLimitedReadDto Reciever { get; set; }
        public TransportReadDto Transport { get; set; }
        public GroupReadDto LimitedTo { get; set; }
    }
}