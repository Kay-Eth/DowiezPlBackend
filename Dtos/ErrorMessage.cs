using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos
{
    public class ErrorMessage
    {
        public ErrorMessage(string title, object details = null)
        {
            Title = title;
            Details = details;
        }

        [Required]
        public string Title { get; set; }
        public object Details { get; set; }
        [Required]
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    }
}