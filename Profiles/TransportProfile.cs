using AutoMapper;
using DowiezPlBackend.Dtos.Transport;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class TransportProfile : Profile
    {
        public TransportProfile()
        {
            CreateMap<Transport, TransportReadDto>();
            CreateMap<TransportCreateDto, Transport>();
            CreateMap<TransportUpdateDto, Transport>();
        }
    }
}