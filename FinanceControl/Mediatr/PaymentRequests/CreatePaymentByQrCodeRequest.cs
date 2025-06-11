using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;


namespace FinanceControl.Mediatr.PaymentRequests;

public class CreatePaymentByQrCodeRequest : IRequest<IActionResult>
{
    [Required]
    public IFormFile File { get; set; }
    
    [JsonIgnore]
    [BindNever]
    [SwaggerIgnore]
    public Guid UserId { get; set; }

    public class CreatePaymentByQrCodeRequestHandler : IRequestHandler<CreatePaymentByQrCodeRequest, IActionResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePaymentByQrCodeRequestHandler> _logger;

        public CreatePaymentByQrCodeRequestHandler(IMediator mediator, ILogger<CreatePaymentByQrCodeRequestHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<IActionResult> Handle(CreatePaymentByQrCodeRequest request, CancellationToken cancellationToken)
        {
            var result = QrUtil.Decode(request.File);
            if (result == null)
            {
                return HttpUtil.GetResponse(HttpStatusCode.BadRequest, "Qr code not found");
            }
          
            var newRequest = new CreatePaymentByUrlRequest()
            {
                UserId = request.UserId,
                Url = result
            };
            return await _mediator.Send(newRequest, cancellationToken);
        }
    }
}