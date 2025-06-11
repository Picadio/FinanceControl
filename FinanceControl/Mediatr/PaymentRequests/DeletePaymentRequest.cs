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

namespace FinanceControl.Mediatr.PaymentRequests;

public class DeletePaymentRequest : IRequest<IActionResult>
{
    public Guid Id { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }

    public class DeletePaymentRequestHandler : IRequestHandler<DeletePaymentRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public DeletePaymentRequestHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(DeletePaymentRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken: cancellationToken);

            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized);
            }
            
            var payment = _mapper.Map<Payment>(request);

            _dbContext.Payments.Attach(payment);

            if (!payment.UserId.Equals(request.UserId))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
            }

            user.Balance += payment.Sum;
            
            _dbContext.Payments.Remove(payment);
            _dbContext.Users.Update(user);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return HttpUtil.SuccessResponse();
        }
    }
}