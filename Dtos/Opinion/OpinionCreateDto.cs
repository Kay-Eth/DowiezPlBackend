using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Opinion
{
    public class OpinionCreateDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }
        
        [Required]
        public Guid RatedId { get; set; }
    }
}