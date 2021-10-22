using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;

namespace DowiezPlBackend.Dtos.Message
{
    public class MessageReadDto
    {
        [Required]
        public Guid MessageId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public AccountLimitedReadDto Sender { get; set; }
    }
}