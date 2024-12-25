using System.Net;
using System.Security.Claims;
using Dumps.Application.DTO.Request.Cart;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class AddToCartCommand : AddToCartRequest, IRequest<APIResponse<Guid>>
    {
    }
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, APIResponse<Guid>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddToCartCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddToCartCommandHandler(AppDbContext context, ILogger<AddToCartCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<Guid>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized.");
                }

                // Get or create the user's cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Bundle)
                    .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);


                if (cart == null)
                {
                    cart = new Dumps.Domain.Entities.Cart { UserId = userId };
                    _context.Carts.Add(cart);
                }

                // Add products to cart
                if (request.ProductIds != null && request.ProductIds.Any())
                {
                    foreach (var productId in request.ProductIds)
                    {
                        // Validate product existence
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted, cancellationToken);

                        if (product == null)
                        {
                            throw new RestException(HttpStatusCode.NotFound, $"Product with ID {productId} not found.");
                        }

                        // Check if the product is already in the cart
                        if (!cart.CartItems.Any(ci => ci.ProductId == productId))
                        {
                            var cartItem = new CartItem
                            {
                                ProductId = productId,
                                CartId = cart.Id
                            };
                            cart.CartItems.Add(cartItem);
                        }
                    }
                }

                // Add bundles to cart
                if (request.BundleIds != null && request.BundleIds.Any())
                {
                    foreach (var bundleId in request.BundleIds)
                    {
                        // Validate bundle existence
                        var bundle = await _context.Bundles
                            .FirstOrDefaultAsync(b => b.Id == bundleId && !b.IsDeleted, cancellationToken);

                        if (bundle == null)
                        {
                            throw new RestException(HttpStatusCode.NotFound, $"Bundle with ID {bundleId} not found.");
                        }

                        // Check if the bundle is already in the cart
                        if (!cart.CartItems.Any(ci => ci.BundleId == bundleId))
                        {
                            var cartItem = new CartItem
                            {
                                BundleId = bundleId,
                                CartId = cart.Id
                            };
                            cart.CartItems.Add(cartItem);
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse<Guid>(cart.Id, "Items added to cart successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding items to cart.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while adding items to cart.");
            }
        }
    }

}
