using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Transport
{
    public class TransportCreateDto
    {
        [Required]
        public DateTime TransportDate { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public Guid StartsInCityId { get; set; }

        [Required]
        public Guid EndsInCityId { get; set; }
    }
}