using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Demand
    {
        [Key]
        public Guid DemandId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public DemandStatus Status { get; set; }

        [Required]
        public DemandCategory Category { get; set; }

        public City From { get; set; }
        
        [Required]
        [ForeignKey("DestinationCityId")]
        public City Destination { get; set; }

        [Required]
        public AppUser Creator { get; set; }

        public AppUser Reciever { get; set; }

        public Transport Transport { get; set; }

        public Group LimitedTo { get; set; }
    }
}