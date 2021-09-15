using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Transport
{
    public class TransportSearchDto
    {
        [Required]
        public ICollection<TransportCategory> Categories { get; set; }
        public Guid? StartsInCityId { get; set; }
        [Required]
        public Guid EndsInCityId { get; set; }
    }
}