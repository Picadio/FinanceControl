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

namespace FinanceControl.Mediatr.UserRequests;

public class UpdateUserBalanceRequest : IRequest<IActionResult>
{
    [Required]
    public double Balance { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserBalanceRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;

        public UpdateUserRequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Handle(UpdateUserBalanceRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken: cancellationToken);
            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized);
            }

            user.Balance = request.Balance;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return HttpUtil.SuccessResponse();
        }
    }
}