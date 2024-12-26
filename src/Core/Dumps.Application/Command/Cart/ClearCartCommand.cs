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
    public class ClearCartCommand : IRequest<APIResponse<string>>
    {
    }

    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, APIResponse<string>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClearCartCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClearCartCommandHandler(AppDbContext context, ILogger<ClearCartCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
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
                    .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

                if (cart == null || !cart.CartItems.Any())
                {
                    return new APIResponse<string>("Cart is already empty.", "No items to clear.");
                }

                // Remove all cart items
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Cart cleared successfully for user {userId}.");
                return new APIResponse<string>("Cart cleared successfully.", "Operation Successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing the cart.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while clearing the cart.");
            }
        }
    }
}
