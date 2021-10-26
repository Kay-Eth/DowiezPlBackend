using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DowiezPlBackend.Enums;

namespace DowiezPlBackend.Models
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public ReportCategory Category { get; set; }

        [Required]
        public ReportStatus Status { get; set; }

        [Required]
        public AppUser Reporter { get; set; }

        public AppUser Reported { get; set; }
        public Transport ReportedTransport { get; set; }
        public Demand ReportedDemand { get; set; }
        public Group ReportedGroup { get; set; }

        public AppUser Operator { get; set; }
    }
}