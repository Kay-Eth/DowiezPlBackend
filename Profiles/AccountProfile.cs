using AutoMapper;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AppUser, AccountReadDto>()
                .ForMember(dest => dest.AccountId,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Role,
                    opts => opts.Ignore());
            
            CreateMap<AccountCreateDto, AppUser>()
                .ForMember(dest => dest.UserName,
                    opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.Banned,
                    opts => opts.Ignore());
                
            CreateMap<AppUser, AccountLimitedReadDto>()
                .ForMember(dest => dest.AccountId,
                    opts => opts.MapFrom(src => src.Id));
        }
    }
}