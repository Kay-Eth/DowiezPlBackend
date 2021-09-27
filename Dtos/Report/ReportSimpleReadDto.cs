using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Report
{
    public class ReportSimpleReadDto
    {
        [Required]
        public Guid ReportId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public ReportCategory Category { get; set; }
        [Required]
        public ReportStatus Status { get; set; }
    }
}