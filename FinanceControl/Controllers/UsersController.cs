using System.ComponentModel;
using FinanceControl.Mediatr.UserRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : AuthorizeController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPatch("updatebalance")]
    public async Task<IActionResult> UpdateBalance([FromBody] UpdateUserBalanceRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var request = new GetUserRequest() { Id = (Guid)userId };
        return await _mediator.Send(request);
    }
}