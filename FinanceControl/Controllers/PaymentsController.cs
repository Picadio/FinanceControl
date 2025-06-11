using FinanceControl.Mediatr.PaymentRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : AuthorizeController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost("createbycheck")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentByCheckRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpPost("createbycheckurl")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentByUrlRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpPost("createbycheckqrcode")]
    public async Task<IActionResult> Create([FromForm] CreatePaymentByQrCodeRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        return await _mediator.Send(request);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentRequest request)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        request.UserId = (Guid)userId;
        request.Id = id;
        return await _mediator.Send(request);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var request = new GetPaymentRequest() {Id = null, Limit = limit, Offset = offset, UserId = (Guid)userId};
        return await _mediator.Send(request);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var request = new GetPaymentRequest() {Id = id, UserId = (Guid)userId};
        return await _mediator.Send(request);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetAuthId();
        if (userId == null) return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var request = new DeletePaymentRequest() { Id = id, UserId = (Guid)userId };
        return await _mediator.Send(request);
    }
}