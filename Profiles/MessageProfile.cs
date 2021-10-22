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
        }
    }
}