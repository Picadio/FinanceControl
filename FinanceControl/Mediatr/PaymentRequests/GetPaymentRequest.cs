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

namespace FinanceControl.Mediatr.PaymentRequests;

public class GetPaymentRequest : IRequest<IActionResult>
{
    [Range(1, 100)]
    public int Limit { get; set; } = 10;
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    [JsonIgnore]
    [BindNever]
    public Guid? Id { get; set; }
    
    public class GetPaymentRequestHandler : IRequestHandler<GetPaymentRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;

        public GetPaymentRequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
        {
            if (request.Id != null)
            {
                var payment = await _dbContext.Payments
                    .FirstOrDefaultAsync(payment =>
                                                    payment.Id == request.Id, cancellationToken: cancellationToken);
                if (payment == default(Payment))
                {
                    return HttpUtil.GetResponse(HttpStatusCode.NotFound, "Payment not found");
                }
                if (payment.UserId != request.UserId)
                {
                    return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
                }
                return HttpUtil.SuccessResponse(payment);
            }
            
            var payments = await _dbContext.Payments
                .Where(payment => payment.UserId == request.UserId)
                .OrderByDescending(payment => payment.DateTime)
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken: cancellationToken);

            var total = await _dbContext.Payments
                .Where(payment => payment.UserId == request.UserId)
                .CountAsync(cancellationToken: cancellationToken);
            
            return HttpUtil.SuccessResponse(new
            {
                Payments = payments,
                Total = total,
                Limit = request.Limit,
                Offset = request.Offset
            });
        }
    }
}