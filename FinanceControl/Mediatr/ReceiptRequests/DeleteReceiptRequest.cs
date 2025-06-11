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

public class DeleteReceiptRequest : IRequest<IActionResult>
{
    [JsonIgnore]
    [BindNever]
    public Guid Id { get; set; }
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class DeleteReceiptRequestHandler : IRequestHandler<DeleteReceiptRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public DeleteReceiptRequestHandler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Handle(DeleteReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = _mapper.Map<Receipt>(request);

            _dbContext.Receipts.Attach(receipt);

            if (!receipt.UserId.Equals(request.UserId))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
            }
            
            _dbContext.Receipts.Remove(receipt);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.SuccessResponse();
        }
    }
}