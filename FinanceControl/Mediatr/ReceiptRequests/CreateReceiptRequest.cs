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


namespace FinanceControl.Mediatr.ReceiptRequests;

public class CreateReceiptRequest : IRequest<IActionResult>
{
    [Length(1, 50)]
    [Required]
    public string Name { get; set; }
    [Required]
    [Range(0, double.MaxValue)]
    public double Sum { get; set; }
    [Range(1, 31)]
    [Required]
    public int Day { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class CreateReceiptRequestHandler : IRequestHandler<CreateReceiptRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateReceiptRequestHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(CreateReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = _mapper.Map<Receipt>(request);

            await _dbContext.Receipts.AddAsync(receipt, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return HttpUtil.GetResponse(HttpStatusCode.Created);
        }
    }
}