using System.Linq;
using AutoMapper;
using DowiezPlBackend.Dtos.Conversation;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationReadDto>();
            CreateMap<Conversation, ConversationDetailedReadDto>()
                .ForMember(dest => dest.ChatMembers,
                    opts => opts.MapFrom(src => src.Participants.Select(p => p.User)));
        }
    }
}