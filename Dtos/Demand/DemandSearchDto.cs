using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Demand
{
    public class DemandSearchDto
    {
        [Required]
        public ICollection<DemandCategory> Categories { get; set; }
        public Guid? FromCityId { get; set; }
        [Required]
        public Guid DestinationCityId { get; set; }
        public Guid? LimitedToGroupId { get; set; }
    }
}