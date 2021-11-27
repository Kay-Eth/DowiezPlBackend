using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.City
{
    public class CityReadDto
    {
        [Required]
        public Guid CityId { get; set; }
        [Required]
        public string CityName { get; set; }
    }
}