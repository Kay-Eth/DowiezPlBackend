using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Report
{
    public class ReportReadDto
    {
        [Required]
        public Guid ReportId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ReportCategory Category { get; set; }
        [Required]
        public ReportStatus Status { get; set; }
        [Required]
        public AccountLimitedReadDto ReporterDto { get; set; }
        public AccountLimitedReadDto ReportedDto { get; set; }

        public AccountLimitedReadDto OperatorDto { get; set; }
    }
}