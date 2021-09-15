using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DowiezPlBackend.Models
{
    public class Transport
    {
        [Key]
        public Guid TransportId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime TransportDate { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public ICollection<Demand> Demands { get; set; }

        [Required]
        public City StartsIn { get; set; }

        [Required]
        public City EndsIn { get; set; }

        [Required]
        public AppUser Creator { get; set; }
    }
}