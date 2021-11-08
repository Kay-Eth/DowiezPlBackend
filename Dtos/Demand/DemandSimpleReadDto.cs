using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Demand
{
    public class DemandSimpleReadDto
    {
        [Required]
        public Guid DemandId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        [Required]
        public DemandCategory Category { get; set; }
        [Required]
        public DemandStatus Status { get; set; }
        public CityReadDto From { get; set; }
        [Required]
        public CityReadDto Destination { get; set; }
    }
}