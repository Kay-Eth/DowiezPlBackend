using AutoMapper;
using DowiezPlBackend.Dtos.Group;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupReadDto>();
            CreateMap<GroupCreateDto, Group>();
            CreateMap<GroupUpdateDto, Group>();
        }
    }
}