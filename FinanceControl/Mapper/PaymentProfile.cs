using AutoMapper;
using FinanceControl.Database.Entities;
using FinanceControl.Mediatr.PaymentRequests;

namespace FinanceControl.Mapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    { 
        CreateMap<CreatePaymentRequest, Payment>(MemberList.Source)
            .ForMember(r => r.DateTime, o => o.MapFrom(p => p.DateTime.ToUniversalTime()));

        CreateMap<DeletePaymentRequest, Payment>(MemberList.Source)
            .ForMember(request => request.Id, o => o.MapFrom(r => r.Id));
        
        CreateMap<GetPaymentRequest, Payment>(MemberList.Source)
            .ForMember(request => request.UserId, o => o.MapFrom(r => r.UserId));
        
        CreateMap<UpdatePaymentRequest, Payment>(MemberList.Source)
            .ForMember(r => r.DateTime, o => o.MapFrom(p => p.DateTime.HasValue ? p.DateTime.Value.ToUniversalTime() : (DateTime?)null)); ;
        
        
    }
}