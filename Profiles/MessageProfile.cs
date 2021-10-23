using AutoMapper;
using DowiezPlBackend.Dtos.Message;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageReadDto>();
            CreateMap<Message, MessageSimpleReadDto>()
                .ForMember(dest => dest.SenderId,
                    opts => opts.MapFrom(src => src.Sender.Id));
        }
    }
}