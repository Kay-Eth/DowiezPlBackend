using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Models
{
    public class City
    {
        [Key]
        public int IdCi { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(4)]
        public string CityName { get; set; }

        [MaxLength(50)]
        [MinLength(4)]
        public string CityDistrict { get; set; }
    }
}