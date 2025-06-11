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

public class CreatePaymentRequest : IRequest<IActionResult>
{
    [Required]
    [Length(1, 50)]
    public string Name { get; set; }
    [Length(0, 500)]
    public string? Description { get; set; }
    [Required]
    [Range(0, double.MaxValue)]
    public double Sum { get; set; }
    [Required]
    public DateTime DateTime { get; set; }
    public DateOnly? MonthlyPayDate { get; set; }
    public string? WhoShouldReturn { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class CreatePaymentRequestHandler : IRequestHandler<CreatePaymentRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreatePaymentRequestHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken: cancellationToken);
            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized);
            }
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(payment => payment.DateTime == request.DateTime.ToUniversalTime() &&
                payment.Sum.Equals(request.Sum), cancellationToken: cancellationToken);
            
            if (payment != default(Payment))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Conflict, "Payment already exists");
            }
            
            payment = _mapper.Map<Payment>(request);

            user.Balance -= payment.Sum;
            
            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.GetResponse(HttpStatusCode.Created);
        }
    }
}