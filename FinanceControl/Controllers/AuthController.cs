using FinanceControl.Mediatr.AuthRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = FinanceControl.Mediatr.AuthRequests.LoginRequest;

namespace FinanceControl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _mediator.Send(request);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    { 
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _mediator.Send(request);
    }
}