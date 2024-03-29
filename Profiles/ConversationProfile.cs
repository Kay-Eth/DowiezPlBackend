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
                    opts => opts.MapFrom(src => src.Participants.Select(p => p.User)))
                .ForMember(dest => dest.LastMessage,
                    opts => opts.MapFrom(src => src.Messages.Count > 0 ? src.Messages.OrderBy(m => m.SentDate).Last() : null));
            CreateMap<Conversation, ConversationSmallDetailReadDto>()
                .ForMember(dest => dest.LastMessage,
                    opts => opts.MapFrom(src => src.Messages.Count > 0 ? src.Messages.OrderBy(m => m.SentDate).Last() : null));
        }
    }
}