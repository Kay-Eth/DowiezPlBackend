using AutoMapper;
using DowiezPlBackend.Dtos.Group;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupReadDto>()
                .ForMember(dest => dest.IsPrivate,
                    opts => opts.MapFrom(src => src.GroupPassword != null)
                );
            CreateMap<GroupCreateDto, Group>();
            CreateMap<GroupUpdateDto, Group>();
        }
    }
}