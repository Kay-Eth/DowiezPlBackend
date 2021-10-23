using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Message
{
    public class MessageSimpleReadDto
    {
        [Required]
        public Guid MessageId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public Guid SenderId { get; set; }
    }
}