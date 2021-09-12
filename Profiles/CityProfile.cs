using AutoMapper;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityReadDto>();
            CreateMap<CityCreateDto, City>();
            CreateMap<CityUpdateDto, City>();
        }
    }
}