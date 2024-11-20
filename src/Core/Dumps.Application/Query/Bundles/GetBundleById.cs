using System.Net;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Bundles
{
    public class GetBundleById
    {
        public class GetBundleByIdQuery : IRequest<APIResponse<CreateBundleResponse>>
        {
            public Guid Id { get; set; }
        }

        public class GetBundleByIdQueryHandler : IRequestHandler<GetBundleByIdQuery, APIResponse<CreateBundleResponse>>
        {
            private readonly AppDbContext _dbContext;
            private readonly ILogger<GetBundleByIdQueryHandler> _logger;

            public GetBundleByIdQueryHandler(AppDbContext dbContext, ILogger<GetBundleByIdQueryHandler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task<APIResponse<CreateBundleResponse>> Handle(GetBundleByIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Filter by Id and ensure the bundle is not deleted
                    var bundle = await _dbContext.Bundles
                        .Where(b => b.Id == request.Id && !b.IsDeleted)
                        .Include(b => b.BundlesProducts)
                        .ThenInclude(bp => bp.Product)
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);

                    if (bundle == null)
                    {
                        throw new RestException(HttpStatusCode.NotFound, "Bundle not found.");
                    }

                    var response = new CreateBundleResponse
                    {
                        Id = bundle.Id,
                        Title = bundle.Title,
                        Description = bundle.Description,
                        DiscountedPrice = bundle.DiscountedPrice,
                        TotalPrice = bundle.BundlesProducts.Sum(bp => bp.Product.Price),
                        Products = bundle.BundlesProducts.Select(bp => new ProductResponse
                        {
                            Id = bp.Product.Id,
                            Title = bp.Product.Title,
                            Description = bp.Product.Description,
                            Price = bp.Product.Price,
                            Discount = bp.Product.Discount,
                            CodeTitle = bp.Product.CodeTitle,
                            CurrentVersion = null
                        }).ToList()
                    };

                    return new APIResponse<CreateBundleResponse>(response, "Bundle retrieved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while retrieving the bundle by Id.");
                    throw new RestException(HttpStatusCode.InternalServerError, "Cannot retrieve bundle.");
                }
            }
        }
    }
}
