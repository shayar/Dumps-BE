using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.DTO.Response.Cart;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Cart
{
    public class GetCartItemByUserId
    {
        public class GetCartItemsByUserIdQuery : IRequest<APIResponse<CartResponse>>
        {
        }

        public class
            GetCartItemsByUserIdQueryHandler : IRequestHandler<GetCartItemsByUserIdQuery, APIResponse<CartResponse>>
        {
            private readonly AppDbContext _dbContext;
            private readonly ILogger<GetCartItemsByUserIdQueryHandler> _logger;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCartItemsByUserIdQueryHandler(AppDbContext dbContext,
                ILogger<GetCartItemsByUserIdQueryHandler> logger, IHttpContextAccessor httpContextAccessor)
            {
                _dbContext = dbContext;
                _logger = logger;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<APIResponse<CartResponse>> Handle(GetCartItemsByUserIdQuery request,
                CancellationToken cancellationToken)
            {
                try
                {
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized.");
                    }

                    // Retrieve the cart with items for the specified user
                    var cart = await _dbContext.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Bundle).ThenInclude(b => b.BundlesProducts)
                        .ThenInclude(bp => bp.Product)
                        .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken).ConfigureAwait(false);

                    if (cart == null || cart.CartItems.Count == 0)
                    {
                        throw new RestException(HttpStatusCode.NotFound, "No cart items found for the user.");
                    }

                    // Map cart items to response
                    var cartResponse = new CartResponse
                    {
                        Id = cart.Id,
                        UserId = cart.UserId,
                        TotalPrice = cart.TotalPrice,
                        Items = cart.CartItems.Select(ci =>
                            ci.ProductId != null
                                ? (object)new ProductResponse
                                {
                                    Id = ci.Product.Id,
                                    Description = ci.Product.Description,
                                    Price = ci.Product.Price,
                                    Discount = ci.Product.Discount,
                                    Title = ci.Product.Title,
                                    CodeTitle = ci.Product.CodeTitle
                                }
                                : (object)new CreateBundleResponse
                                {
                                    Id = ci.Id,
                                    Title = ci.Bundle.Title,
                                    Description = ci.Bundle.Description,
                                    DiscountedPrice = ci.Bundle.DiscountedPrice,
                                    TotalPrice = ci.Bundle.BundlesProducts.Sum(bp => bp.Product.Price),
                                    Products = ci.Bundle.BundlesProducts.Select(bp => new ProductResponse
                                    {
                                        Id = bp.Product.Id,
                                        Title = bp.Product.Title,
                                        Description = bp.Product.Description,
                                        Price = bp.Product.Price,
                                        Discount = bp.Product.Discount,
                                        CodeTitle = bp.Product.CodeTitle,
                                        CurrentVersion = null
                                    }).ToList()
                                }
                        ).ToList()
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
