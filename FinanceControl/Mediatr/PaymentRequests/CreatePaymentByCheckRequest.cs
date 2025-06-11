using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using FinanceControl.Database;
using FinanceControl.Database.Entities;
using FinanceControl.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinanceControl.Mediatr.PaymentRequests;

public class CreatePaymentByCheckRequest : IRequest<IActionResult>
{
    [Required]
    public string Fn { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public double Sum { get; set; }
    [Required]
    public string Id { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }
    
    public class CreateByCheckRequestHandler : IRequestHandler<CreatePaymentByCheckRequest, IActionResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateByCheckRequestHandler> _logger;
        private readonly IMediator _mediator;

        public CreateByCheckRequestHandler(AppDbContext dbContext, HttpClient httpClient, ILogger<CreateByCheckRequestHandler> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _logger = logger;
            _mediator = mediator;
        }
        
        public async Task<IActionResult> Handle(CreatePaymentByCheckRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken: cancellationToken);

            if (user == default(User))
            {
                return HttpUtil.GetResponse(HttpStatusCode.Unauthorized);
            }
            
            var response = await _httpClient.GetAsync(HttpUtil.GetUrlApiTaxGovUa(request.Sum, request.Date, request.Fn, request.Id), cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning(content);
                return HttpUtil.GetResponse(response.StatusCode, "Error from tax gov ua api");
            }
            
            var json = JObject.Parse(content);
            var base64Check = json["check"]?.ToString();
            
            var checkBytes = Convert.FromBase64String(base64Check);
            var decodedCheck = Encoding.UTF8.GetString(checkBytes);

            var newRequest = new CreatePaymentRequest()
            {
                DateTime = request.Date,
                MonthlyPayDate = null,
                Name = "Чек " + request.Id,
                Description = decodedCheck,
                Sum = request.Sum,
                UserId = request.UserId,
                WhoShouldReturn = null
            };
            
            return await _mediator.Send(newRequest, cancellationToken);
        }
    }
}