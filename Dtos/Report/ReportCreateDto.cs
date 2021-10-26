using System;
using System.ComponentModel.DataAnnotations;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Dtos.Report
{
    public class ReportCreateDto
    {
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public ReportCategory Category { get; set; }

        public Guid? ReportedId { get; set; }
        public Guid? ReportedTransportId { get; set; }
        public Guid? ReportedDemandId { get; set; }
        public Guid? ReportedGroupId { get; set; }
    }
}