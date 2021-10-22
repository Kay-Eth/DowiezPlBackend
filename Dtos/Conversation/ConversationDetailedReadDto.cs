using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Dtos.Message;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Conversation
{
    public class ConversationDetailedReadDto
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public ConversationCategory Category { get; set; }

        [Required]
        public ICollection<AccountLimitedReadDto> ChatMembers { get; set; }

        [Required]
        public MessageReadDto LastMessage { get; set; }
    }
}