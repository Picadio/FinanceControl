using FinanceControl.Mediatr.UserRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : AuthorizeController
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
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
}