using System;
using AutoMapper;
using DowiezPlBackend.Dtos.Report;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Report, ReportReadDto>();
            CreateMap<Report, ReportSimpleReadDto>()
                .ForMember(dest => dest.Assigned,
                    opts => opts.MapFrom(src => src.Operator != null));
            CreateMap<ReportCreateDto, Report>();
            CreateMap<ReportUpdateDto, Report>();
        }
    }
}