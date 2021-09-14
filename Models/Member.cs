using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Member
    {
        [Key]
        public Guid MemberId { get; set; }

        [Required]
        public AppUser User { get; set; }

        [Required]
        public Group Group { get; set; }
    }
}