using AutoMapper;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace DowiezPlBackend.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AppUser, AccountReadDto>()
                .ForMember(dest => dest.AccountId,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email,
                    opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role,
                    opts => opts.Ignore())
                .ForMember(dest => dest.FirstName,
                    opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName,
                    opts => opts.MapFrom(src => src.LastName));
            
            CreateMap<AccountCreateDto, AppUser>()
                .ForMember(dest => dest.Email,
                    opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName,
                    opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName,
                    opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName,
                    opts => opts.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Banned,
                    opts => opts.Ignore());
        }
    }
}