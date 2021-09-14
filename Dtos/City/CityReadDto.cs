using System;
using System.ComponentModel.DataAnnotations;

namespace DowiezPlBackend.Dtos.City
{
    public class CityReadDto
    {
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public string CityDistrict { get; set; }
    }
}