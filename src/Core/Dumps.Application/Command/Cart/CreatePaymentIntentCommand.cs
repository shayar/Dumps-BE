using System.Net;
using System.Security.Claims;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class CreatePaymentIntentCommand : IRequest<APIResponse<string>>
    {
    }

    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, APIResponse<string>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreatePaymentIntentCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;

        public CreatePaymentIntentCommandHandler(AppDbContext context,
            ILogger<CreatePaymentIntentCommandHandler> logger, IHttpContextAccessor httpContextAccessor,
            IPaymentService paymentService)
        {
            _context = context;
            _logger = logger;
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> Handle(CreatePaymentIntentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Fetch the UserId from HttpContext
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized.");
                }

                // Fetch the user's cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Bundle)
                    .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

                if (cart == null || cart.CartItems.Count == 0)
                {
                    return new APIResponse<string>("Cart is already empty.", "No items to clear.");
                }

                // Create Payment Intent
                string clientSecret =
                    await _paymentService.CreatePaymentIntent((long)Convert.ToInt64(Math.Round(cart.TotalPrice * 100)), userId);

                _logger.LogInformation($"Cart cleared successfully for user {userId}.");
                return new APIResponse<string>(clientSecret, "Payment Intent Created Successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating payment intent.");
                throw new RestException(HttpStatusCode.InternalServerError,
                    "An error occurred while creating payment intent.");
            }
        }
    }
}
