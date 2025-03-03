using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Application.Command.Cart
{
    public class RemovePromoCodeCommand : IRequest<APIResponse<string>>
    {
    }

    public class RemovePromoCodeCommandHandler : IRequestHandler<RemovePromoCodeCommand, APIResponse<string>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RemovePromoCodeCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RemovePromoCodeCommandHandler(AppDbContext context, ILogger<RemovePromoCodeCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> Handle(RemovePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized.");
                }

                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

                if (cart == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, "Cart not found.");
                }

                if (cart.AppliedPromoCode == null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, "No promo code applied.");
                }

                // Remove the applied promo code and reset discount
                cart.AppliedPromoCode = null;
                cart.PromoDiscount = 0;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse<string>("Promo code removed successfully.", "Operation successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing promo code.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while removing promo code.");
            }
        }
    }
}
