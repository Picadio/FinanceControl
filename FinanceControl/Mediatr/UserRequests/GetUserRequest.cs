using FinanceControl.Database;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Mediatr.UserRequests;

public class GetUserRequest : IRequest<IActionResult>
{
    public Guid Id { get; set; }

    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;

        public GetUserRequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstAsync(user => user.Id == request.Id, cancellationToken: cancellationToken);
            
            return HttpUtil.SuccessResponse(user);

        }
    }
}