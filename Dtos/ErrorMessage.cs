using System;

namespace DowiezPlBackend.Dtos
{
    public class ErrorMessage
    {
        public ErrorMessage(string title, object details = null)
        {
            Title = title;
            Details = details;
        }

        public string Title { get; set; }
        public object Details { get; set; }
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    }
}