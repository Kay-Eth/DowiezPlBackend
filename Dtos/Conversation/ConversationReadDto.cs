using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Conversation
{
    public class ConversationReadDto
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public ConversationCategory Category { get; set; }
    }
}