using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using FinanceControl.Database;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinanceControl.Mediatr.PaymentRequests;

public class CreatePaymentByUrlRequest : IRequest<IActionResult>
{
    [Required]
    public string Url { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }

    public class CreatePaymentByUrlRequestHandler : IRequestHandler<CreatePaymentByUrlRequest, IActionResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePaymentByUrlRequestHandler> _logger;
        private const string Format = "yyyyMMddHHmm";
        
        public CreatePaymentByUrlRequestHandler(IMediator mediator, ILogger<CreatePaymentByUrlRequestHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(CreatePaymentByUrlRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri(request.Url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            var fn = query["fn"];
            var date = query["date"];
            var time = query["time"];
            var id = query["id"];
            var sm = query["sm"];

            if (fn == null || date == null || time == null || id == null || sm == null)
            {
                return HttpUtil.GetResponse(HttpStatusCode.BadRequest, "Check is not valid");
            }

            if (time.Length > 4)
            {
                time = time.Substring(0, 4);
            }
            
            var dateTime =
                DateTime.ParseExact(date + time, Format, CultureInfo.InvariantCulture, DateTimeStyles.None);
            
            var sum = double.Parse(sm, CultureInfo.InvariantCulture);

            var newRequest = new CreatePaymentByCheckRequest()
            {
                UserId = request.UserId,
                Fn = fn,
                Date = dateTime,
                Id = id,
                Sum = sum
            };
            return await _mediator.Send(newRequest, cancellationToken);
        }
    }
}