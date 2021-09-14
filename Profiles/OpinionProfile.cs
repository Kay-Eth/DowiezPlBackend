using AutoMapper;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Dtos.Opinion;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class OpinionProfile : Profile
    {
        public OpinionProfile()
        {
            CreateMap<Opinion, OpinionReadDto>()
                .ForMember(dest => dest.Issuer,
                    opts => opts.Ignore())
                .ForMember(dest => dest.Rated,
                    opts => opts.Ignore());
            
            CreateMap<OpinionCreateDto, Opinion>();
            CreateMap<OpinionUpdateDto, Opinion>();
        }
    }
}