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

public class EnableReceiptRequest : IRequest<IActionResult>
{
    [Required]
    public bool Enable { get; set; }
    
    [BindNever]
    [JsonIgnore]
    public Guid Id { get; set; }
    [BindNever]
    [JsonIgnore]
    public Guid UserId { get; set; }

    public class EnableReceiptRequestHandler : IRequestHandler<EnableReceiptRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;

        public EnableReceiptRequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Handle(EnableReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(receipt => receipt.Id == request.Id, cancellationToken: cancellationToken);
            if (receipt == default(Receipt))
            {
                return HttpUtil.GetResponse(HttpStatusCode.NotFound, "Receipt not found");
            }
            if (receipt.UserId != request.UserId)
            {
                return HttpUtil.GetResponse(HttpStatusCode.Forbidden);
            }

            receipt.Enabled = request.Enable;
                
            _dbContext.Receipts.Update(receipt);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.SuccessResponse();
        }
    }
}