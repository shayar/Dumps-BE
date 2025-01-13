using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class RemoveCartItemCommand : IRequest<APIResponse<string>>
    {
        public Guid? ProductId { get; set; } // Product ID to be removed
        public Guid? BundleId { get; set; } // Bundle ID to be removed
    }
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, APIResponse<string>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RemoveCartItemCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RemoveCartItemCommandHandler(AppDbContext context, ILogger<RemoveCartItemCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
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
                    return new APIResponse<string>("Cart is empty or does not exist.", "No items to remove.");
                }

                // Find the cart item to remove based on ProductId or BundleId
                var itemToRemove = cart.CartItems.FirstOrDefault(ci =>
                    (request.ProductId.HasValue && ci.ProductId == request.ProductId) ||
                    (request.BundleId.HasValue && ci.BundleId == request.BundleId));

                if (itemToRemove == null)
                {
                    return new APIResponse<string>("No matching item found in the cart.", "Nothing to remove.");
                }

                // Remove the item from the cart
                _context.CartItems.Remove(itemToRemove);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Removed item with ID {itemToRemove.Id} from the cart for user {userId}.");

                return new APIResponse<string>("Item removed successfully.", "Operation Successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item from the cart.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while removing item from the cart.");
            }
        }

    }
}
