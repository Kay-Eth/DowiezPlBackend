using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Demand
{
    public class DemandSearchDto
    {
        [Required]
        ICollection<DemandCategory> Categories { get; set; }
        Guid? FromCityId { get; set; }
        [Required]
        Guid DestinationCityId { get; set; }
        Guid? LimitedToGroupId { get; set; }
    }
}