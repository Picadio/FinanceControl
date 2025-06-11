using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;
using AutoMapper;
using FinanceControl.Database;
using FinanceControl.Database.Entities;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Mediatr.ReceiptRequests;

public class UpdateReceiptRequest : IRequest<IActionResult>
{
    [Length(1, 50)]
    public string Name { get; set; }
    [Range(0, double.MaxValue)]
    public double Sum { get; set; }
    [Range(1, 31)]
    public int Day { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid Id { get; set; }
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }

    public class UpdateReceiptRequestHandler : IRequestHandler<UpdateReceiptRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateReceiptRequestHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(UpdateReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(receipt => receipt.Id == request.Id, cancellationToken: cancellationToken);
            if (receipt == default(Receipt))
            {
                return HttpUtil.GetResponse(HttpStatusCode.NotFound);
            }
            if (!receipt.UserId.Equals(request.UserId))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
            }
            
            receipt = _mapper.Map(request, receipt);
            
            _dbContext.Receipts.Update(receipt);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.SuccessResponse();
        }
    }
}