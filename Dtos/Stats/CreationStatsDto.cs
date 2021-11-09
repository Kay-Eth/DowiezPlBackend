using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.Stats
{
    public class CreationStatsDto
    {
        public DateTime CreationDate { get; set; }
        public Dictionary<string, int> Users { get; set; }
        public Dictionary<string, int> Demands { get; set; }
        public Dictionary<string, int> Transports { get; set; }
    }
}