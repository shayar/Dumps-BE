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
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);


                if (cart == null)
                {
                    cart = new Dumps.Domain.Entities.Cart { UserId = userId };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Add products to cart
                if (request.ProductId.HasValue)
                {
                    // Validate product existence
                    var productExists = await _context.Products
               .AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

                    if (!productExists)
                    {
                        throw new RestException(HttpStatusCode.NotFound, $"Product with ID {request.ProductId} not found.");
                    }

                    // Check if the product is already in the cart
                    if (!cart.CartItems.Any(ci => ci.ProductId == request.ProductId))
                    {
                        var cartItem = new CartItem
                        {
                            ProductId = request.ProductId,
                            CartId = cart.Id
                        };
                        _context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        throw new RestException(HttpStatusCode.Conflict, "Product already exists in cart.");
                    }
                }

                // Add bundles to cart
                else if (request.BundleId.HasValue)
                {
                    var bundleExists = await _context.Bundles
                        .AnyAsync(b => b.Id == request.BundleId && !b.IsDeleted, cancellationToken);

                    if (!bundleExists)
                    {
                        throw new RestException(HttpStatusCode.NotFound, $"Bundle with ID {request.BundleId} not found.");
                    }

                    if (!cart.CartItems.Any(ci => ci.BundleId == request.BundleId))
                    {
                        var cartItem = new CartItem
                        {

                            BundleId = request.BundleId,
                            CartId = cart.Id
                        };
                        _context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        throw new RestException(HttpStatusCode.Conflict, "Bundle already exists in cart.");
                    }
                }
                else
                {
                    throw new RestException(HttpStatusCode.BadRequest, "Either ProductId or BundleId must be provided.");
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
