using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Models
{
    public class Participant
    {
        [Key]
        public Guid ParticipantId { get; set; }

        [Required]
        public AppUser User { get; set; }

        [Required]
        public Conversation Conversation { get; set; }
    }
}