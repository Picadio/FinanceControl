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

namespace FinanceControl.Mediatr.PaymentRequests;

public class UpdatePaymentRequest : IRequest<IActionResult>
{
    [Length(1, 50)]
    public string? Name { get; set; }
    [Length(0, 500)]
    public string? Description { get; set; }
    [Range(0, double.MaxValue)]
    public double? Sum { get; set; }
    public DateTime? DateTime { get; set; }
    public DateOnly? MonthlyPayDate { get; set; }
    [Length(1, 50)]
    public string? WhoShouldReturn { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid Id { get; set; }
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class UpdatePaymentResponseHandler : IRequestHandler<UpdatePaymentRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdatePaymentResponseHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(UpdatePaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(payment => payment.Id == request.Id, cancellationToken: cancellationToken);
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken: cancellationToken);

            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized);
            }
            if (payment == default(Payment))
            {
                return HttpUtil.GetResponse(HttpStatusCode.NotFound);
            }
            if (!payment.UserId.Equals(request.UserId))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
            }

            if (payment.Sum.Equals(request.Sum))
            {
                user.Balance += payment.Sum - (double)request.Sum;
            }
            
            payment = _mapper.Map(request, payment);
            
            _dbContext.Payments.Update(payment);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.SuccessResponse();
        }
    }
}