using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        [MaxLength(500)]
        [MinLength(1)]
        public string Content { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public AppUser Sender { get; set; }

        [Required]
        public Conversation Conversation { get; set; }
    }
}