using AutoMapper;
using FinanceControl.Database.Entities;
using FinanceControl.Mediatr.ReceiptRequests;

namespace FinanceControl.Mapper;

public class ReceiptProfile : Profile
{
    public ReceiptProfile()
    {
        CreateMap<CreateReceiptRequest, Receipt>();
        CreateMap<UpdateReceiptRequest, Receipt>();
        CreateMap<DeleteReceiptRequest, Receipt>();
    }
}