using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Group;
using DowiezPlBackend.Dtos.Message;
using DowiezPlBackend.Dtos.Transport;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Conversation
{
    public class ConversationSmallDetailReadDto
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public ConversationCategory Category { get; set; }

        [Required]
        public string Name { get; set; }

        public MessageReadDto LastMessage { get; set; }
    }
}