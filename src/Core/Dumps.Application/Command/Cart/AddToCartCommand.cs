using System.Net;
using Dumps.Application.DTO.Request.Cart;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Cart
{
    public class AddToCartCommand : AddToCartRequest, IRequest<APIResponse<Guid>>
    {
        public string UserId { get; set; }
    }
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, APIResponse<Guid>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddToCartCommandHandler> _logger;

        public AddToCartCommandHandler(AppDbContext context, ILogger<AddToCartCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<APIResponse<Guid>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;

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

                foreach (var item in request.Items)
                {
                    if (item.ProductId == null && item.BundleId == null)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, "Each item must have either a ProductId or BundleId.");
                    }

                    if (item.ProductId != null && !await _context.Products.AnyAsync(p => p.Id == item.ProductId && !p.IsDeleted, cancellationToken))
                    {
                        throw new RestException(HttpStatusCode.NotFound, $"Product with ID {item.ProductId} not found.");
                    }

                    if (item.BundleId != null && !await _context.Bundles.AnyAsync(b => b.Id == item.BundleId && !b.IsDeleted, cancellationToken))
                    {
                        throw new RestException(HttpStatusCode.NotFound, $"Bundle with ID {item.BundleId} not found.");
                    }

                    var cartItem = new CartItem
                    {
                        ProductId = item.ProductId,
                        BundleId = item.BundleId,
                    };

                    cart.CartItems.Add(cartItem);
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
