using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [MaxLength(200)]
        [MinLength(4)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(256)]
        [MinLength(4)]
        public string GroupPassword { get; set; }

        public ICollection<Demand> LimitedBy { get; set; }
        public ICollection<Member> Members { get; set; }

        [Required]
        public AppUser Creator { get; set; }

        [Required]
        [ForeignKey("ConversationId")]
        public Conversation GroupConversation { get; set; }
    }
}