using FinanceControl.Mediatr.ReceiptRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReceiptsController : AuthorizeController
{
    private readonly IMediator _mediator;

    public ReceiptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet()]
    public async Task<IActionResult> Get([FromQuery] GetReceiptRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var request = new GetReceiptRequest() { Id = id, UserId = (Guid)userId };
        return await _mediator.Send(request);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateReceiptRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Enable(Guid id, [FromBody] EnableReceiptRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        request.Id = id;
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReceiptRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        request.Id = id;
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var request = new DeleteReceiptRequest() { Id = id, UserId = (Guid)userId };
        return await _mediator.Send(request);
    }
}