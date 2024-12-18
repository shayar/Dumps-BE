using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.DTO.Response.Cart;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Cart
{
    public class GetCartItemByUserId
    {
        public class GetCartItemsByUserIdQuery : IRequest<APIResponse<CartResponse>>
        {
            public string UserId { get; set; }
        }

        public class GetCartItemsByUserIdQueryHandler : IRequestHandler<GetCartItemsByUserIdQuery, APIResponse<CartResponse>>
        {
            private readonly AppDbContext _dbContext;
            private readonly ILogger<GetCartItemsByUserIdQueryHandler> _logger;

            public GetCartItemsByUserIdQueryHandler(AppDbContext dbContext, ILogger<GetCartItemsByUserIdQueryHandler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task<APIResponse<CartResponse>> Handle(GetCartItemsByUserIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Retrieve the cart with items for the specified user
                    var cart = await _dbContext.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Bundle)
                        .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

                    if (cart == null || !cart.CartItems.Any())
                    {
                        throw new RestException(HttpStatusCode.NotFound, "No cart items found for the user.");
                    }

                    // Map cart items to response
                    var cartResponse = new CartResponse
                    {
                        CartId = cart.Id,
                        UserId = cart.UserId,
                        TotalPrice = cart.TotalPrice,
                        CartItems = cart.CartItems.Select(ci => new CartItemResponse
                        {
                            CartItemId = ci.Id,
                            ProductId = ci.ProductId,
                            ProductTitle = ci.Product?.Title,
                            ProductPrice = ci.Product?.Price,
                            BundleId = ci.BundleId,
                            BundleTitle = ci.Bundle?.Title,
                            ItemPrice = ci.ItemPrice
                        }).ToList()
                    };

                    return new APIResponse<CartResponse>(cartResponse, "Cart items retrieved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while retrieving cart items.");
                    throw new RestException(HttpStatusCode.InternalServerError, "Cannot retrieve cart items.");
                }
            }

        }
    }
}

