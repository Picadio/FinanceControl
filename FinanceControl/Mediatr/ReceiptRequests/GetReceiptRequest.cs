using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;
using FinanceControl.Database;
using FinanceControl.Database.Entities;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;


namespace FinanceControl.Mediatr.ReceiptRequests;

public class GetReceiptRequest : IRequest<IActionResult>
{
    [Range(1, 100)]
    [DefaultValue(10)]
    public int Limit { get; set; } = 10;
    [Range(0, int.MaxValue)]
    [DefaultValue(0)]
    public int Offset { get; set; } = 0;
    
    [JsonIgnore]
    [BindNever]
    public Guid? Id { get; set; }
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }

    public class GetReceiptRequestHandler : IRequestHandler<GetReceiptRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;

        public GetReceiptRequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Handle(GetReceiptRequest request, CancellationToken cancellationToken)
        {
            if (request.Id != null)
            {
                var receipt = await _dbContext.Receipts
                    .FirstOrDefaultAsync(receipt =>
                                                    receipt.Id == request.Id, cancellationToken: cancellationToken);
                if (receipt == default(Receipt))
                {
                    return HttpUtil.GetResponse(HttpStatusCode.NotFound, "Payment not found");
                }
                if (receipt.UserId != request.UserId)
                {
                    return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
                }
                return HttpUtil.SuccessResponse(receipt);
            }
          
            var receipts = await _dbContext.Receipts
                .Where(payment => payment.UserId == request.UserId)
                .Where(payment => payment.Id == request.Id)
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken: cancellationToken);
            return HttpUtil.SuccessResponse(receipts);
        }
    }
}