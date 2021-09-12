using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.City
{
    public class CityCreateDto
    {
        [Required]
        [MaxLength(150)]
        [MinLength(4)]
        public string CityName { get; set; }
        
        [MaxLength(50)]
        [MinLength(4)]
        public string CityDistrict { get; set; }
    }
}