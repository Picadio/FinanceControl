using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using FinanceControl.Database;
using FinanceControl.Database.Entities;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Mediatr.AuthRequests;

public class RegisterRequest : IRequest<IActionResult>
{
    [Required]
    [Length(1, 30)]
    public string Login { get; set; }
    [Required]
    [Length(1, 30)]
    public string Password { get; set; }
    
    public class RegisterRequestHandler : IRequestHandler<RegisterRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RegisterRequestHandler(AppDbContext dbContext, IMapper _mapper)
        {
            _dbContext = dbContext;
            this._mapper = _mapper;
        }
        
        public async Task<IActionResult> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Login, cancellationToken: cancellationToken);
            if (user != default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Conflict, "User already exists");
            }

            user = _mapper.Map<User>(request);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return HttpUtil.SuccessResponse();
        }
    }
}