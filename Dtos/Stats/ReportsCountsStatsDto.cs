using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Stats
{
    public class ReportsCountsStatsDto
    {
        [Required]
        public int PackageCount { get; set; }
        [Required]
        public int UserCount { get; set; }
        [Required]
        public int TechnicalCount { get; set; }
        [Required]
        public int OpinionCount { get; set; }
    }
}