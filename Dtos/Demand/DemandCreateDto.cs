using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Demand
{
    public class DemandCreateDto
    {
        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public DemandCategory Category { get; set; }

        public Guid FromCityId { get; set; }
        
        [Required]
        public Guid DestinationCityId { get; set; }

        public Guid? RecieverUserId { get; set; }

        public Guid? LimitedToGroupId { get; set; }
    }
}