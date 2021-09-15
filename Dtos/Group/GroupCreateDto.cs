using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Group
{
    public class GroupCreateDto
    {
        [Required]
        [MaxLength(200)]
        [MinLength(4)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
    }
}