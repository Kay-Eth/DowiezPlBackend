using AutoMapper;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class DemandProfile : Profile
    {
        public DemandProfile()
        {
            CreateMap<Demand, DemandReadDto>();
            CreateMap<Demand, DemandSimpleReadDto>();
            CreateMap<DemandCreateDto, Demand>();
            CreateMap<DemandUpdateDto, Demand>();
        }
    }
}