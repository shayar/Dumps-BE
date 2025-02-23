using System.Net;
using System.Security.Claims;
using Dumps.Application.DTO.Request.Cart;
using Dumps.Application.Exceptions;
using Dumps.Domain.Common.Enums;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class ApplyPromoCodeCommand : ApplyPromoCodeRequest, IRequest<APIResponse<decimal>>
    {
    }

    public class ApplyPromoCodeCommandHandler : IRequestHandler<ApplyPromoCodeCommand, APIResponse<decimal>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ApplyPromoCodeCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplyPromoCodeCommandHandler(AppDbContext context, ILogger<ApplyPromoCodeCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<decimal>> Handle(ApplyPromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized.");
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

                if (cart == null || !cart.CartItems.Any())
                {
                    throw new RestException(HttpStatusCode.NotFound, "Cart is empty.");
                }

                // Validate the promo code
                var promo = await _context.PromoCodes
                    .FirstOrDefaultAsync(p => p.Code == request.PromoCode && p.IsActive, cancellationToken);

                if (promo == null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, "Invalid or expired promo code.");
                }

                // Ensure the promo code is not already applied
                if (cart.AppliedPromoCode == request.PromoCode)
                {
                    throw new RestException(HttpStatusCode.BadRequest, "Promo code is already applied.");
                }

                // Calculate discount
                decimal discount = 0;
                decimal cartTotal = cart.CartItems.Sum(ci => ci.Product.Price);
                switch (promo.DiscountType)
                {
                    case DiscountType.Percentage:
                        discount = cartTotal * promo.DiscountValue / 100;
                        break;
                    case DiscountType.Flat:
                        discount = promo.DiscountValue;
                        break;
                }

                discount = Math.Min(discount, promo.MaxDiscount);

                // Store the applied promo code and discount in the cart
                cart.AppliedPromoCode = promo.Code;
                cart.PromoDiscount = discount;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse<decimal>(discount, "Promo code applied successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while applying promo code.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while applying promo code.");
            }
        }
    }
}
