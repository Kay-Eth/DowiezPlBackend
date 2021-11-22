using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DowiezPlBackend.Models
{
    public class Opinion
    {
        [Key]
        public Guid OpinionId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }
        
        [Required]
        public AppUser Issuer { get; set; }

        [Required]
        public AppUser Rated { get; set; }
    }
}