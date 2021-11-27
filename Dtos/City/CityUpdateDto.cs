using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.City
{
    public class CityUpdateDto
    {
        [Key]
        public Guid CityId { get; set; }
        
        [Required]
        [MaxLength(150)]
        [MinLength(4)]
        public string CityName { get; set; }
    }
}