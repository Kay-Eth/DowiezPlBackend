using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Report
{
    public class ReportUpdateDto
    {
        [Key]
        public Guid ReportId { get; set; }
        
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public ReportCategory Category { get; set; }
    }
}