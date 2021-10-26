using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Stats
{
    public class ReportsCountsStatsDto
    {
        [Required]
        public int UserCount { get; set; }
        [Required]
        public int TransportCount { get; set; }
        [Required]
        public int DemandCount { get; set; }
        [Required]
        public int GroupCount { get; set; }
        [Required]
        public int TechnicalCount { get; set; }
    }
}