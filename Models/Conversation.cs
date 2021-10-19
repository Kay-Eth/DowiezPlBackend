using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Conversation
    {
        [Key]
        public Guid ConversationId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public ConversationCategory Category { get; set; }

        public Group OwnerGroup { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }
}