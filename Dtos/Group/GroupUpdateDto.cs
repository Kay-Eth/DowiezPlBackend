using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Group
{
    public class GroupUpdateDto
    {
        [Key]
        public Guid GroupId { get; set; }
        
        [Required]
        [MaxLength(200)]
        [MinLength(4)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(256)]
        [MinLength(4)]
        public string GroupPassword { get; set; }
    }
}