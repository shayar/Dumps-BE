using System.Net;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class PaymentWebhookCommand : IRequest<Unit>
    {
    }

    public class PaymentWebhookCommandHandler : IRequestHandler<PaymentWebhookCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentWebhookCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;

        public PaymentWebhookCommandHandler(AppDbContext context,
            ILogger<PaymentWebhookCommandHandler> logger, IHttpContextAccessor httpContextAccessor,
            IPaymentService paymentService)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        public async Task<Unit> Handle(PaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            var httpRequest = _httpContextAccessor.HttpContext?.Request;

            if (httpRequest == null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid webhook request.");

            string json;
            using (var reader = new StreamReader(httpRequest.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            try
            {
                var stripeSignature = httpRequest.Headers["Stripe-Signature"];
                await _paymentService.HandleStripeWebhookAsync(json, stripeSignature);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed.");
                throw new RestException(HttpStatusCode.InternalServerError, "Unexpected error processing webhook.");
            }
        }
    }
}
