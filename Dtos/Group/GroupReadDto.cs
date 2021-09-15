using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Group
{
    public class GroupReadDto
    {
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        // TODO: Conversation dto (or maybe not)
    }
}