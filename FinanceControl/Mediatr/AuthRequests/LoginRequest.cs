using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using FinanceControl.Database;
using FinanceControl.Database.Entities;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinanceControl.Mediatr.AuthRequests;

public class LoginRequest : IRequest<IActionResult>
{
    [Required]
    [Length(1, 30)]
    public string Login { get; set; }
    [Required]
    [Length(1, 30)]
    public string Password { get; set; }
    
    public class LoginRequestHandler : IRequestHandler<LoginRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;

        public LoginRequestHandler(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        
        public async Task<IActionResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Login, cancellationToken: cancellationToken);
            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized, "Incorrect credentials");
            }

            if (HashUtil.VerifyHash(user.PasswordHash, request.Password))
            {
                var jwtSettings = _config.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Login),
                    new Claim("UserId", user.Id.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds
                );

                return HttpUtil.SuccessResponse(new JwtSecurityTokenHandler().WriteToken(token));
            }

            return HttpUtil.GetResponse(HttpStatusCode.Unauthorized, "Incorrect credentials");
        }
    }
}