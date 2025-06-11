using AutoMapper;
using FinanceControl.Database.Entities;
using FinanceControl.Mediatr.AuthRequests;
using FinanceControl.Utils;

namespace FinanceControl.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<LoginRequest, User>(MemberList.Source)
            .ForMember(u => u.Name, o => o.MapFrom(r => r.Login))
            .ForMember(u => u.PasswordHash, o => o.MapFrom(r => HashUtil.Hash(r.Password)));
        
        CreateMap<RegisterRequest, User>(MemberList.Source)
            .ForMember(u => u.Name, o => o.MapFrom(r => r.Login))
            .ForMember(u => u.PasswordHash, o => o.MapFrom(r => HashUtil.Hash(r.Password)));
    }
}