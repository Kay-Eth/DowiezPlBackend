using System;

namespace DowiezPlBackend.Exceptions
{
    public class DowiezPlException : Exception
    {
        public DowiezPlException() : base() {}
        public DowiezPlException(string message) : base(message) {}

        public virtual int StatusCode { get; set; } = 500;

        public virtual string Detail { get; set; } = null;
    }
}