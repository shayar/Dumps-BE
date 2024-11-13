using System.Linq.Expressions;
using System.Net;
using Dumps.Application.APIResponse;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Bundles
{
    public class GetAllBundles
    {
        public class GetAllBundlesQuery : IRequest<APIResponse<IList<CreateBundleResponse>>>
        {
        }

        public class GetAllBundlesQueryHandler : IRequestHandler<GetAllBundlesQuery, APIResponse<IList<CreateBundleResponse>>>
        {
            private readonly AppDbContext _dbContext;
            private readonly ILogger<GetAllBundlesQueryHandler> _logger;

            public GetAllBundlesQueryHandler(AppDbContext dbContext, ILogger<GetAllBundlesQueryHandler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task<APIResponse<IList<CreateBundleResponse>>> Handle(GetAllBundlesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Filter only non-deleted bundles
                    Expression<Func<Bundle, bool>> filter = x => !x.IsDeleted;

                    var bundlesList = await _dbContext.Bundles
                        .Where(filter)
                        .Include(b => b.BundlesProducts)
                        .ThenInclude(bp => bp.Product)
                        .OrderByDescending(b => b.CreatedAt)
                        .Select(b => new CreateBundleResponse
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Description = b.Description,
                            DiscountedPrice = b.DiscountedPrice,
                            ProductIds = b.BundlesProducts.Select(bp => bp.ProductId).ToList()
                        })
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new APIResponse<IList<CreateBundleResponse>>(bundlesList, "Bundles retrieved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while retrieving all bundles.");
                    throw new RestException(HttpStatusCode.InternalServerError, "Cannot retrieve bundles.");
                }
            }
        }
    }
}
