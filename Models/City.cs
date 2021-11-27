using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Models
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(4)]
        public string CityName { get; set; }
    }
}