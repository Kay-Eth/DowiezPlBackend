using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.City
{
    public class CityUpdateDto
    {
        [Key]
        public int IdCi { get; set; }
        
        [Required]
        [MaxLength(150)]
        [MinLength(4)]
        public string CityName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string CityDistrict { get; set; }
    }
}