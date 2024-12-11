using System.Linq.Expressions;
using System.Net;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.DTO.Response.Products;
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
            public string? Sort { get; }
            public int Page { get; }
            public int Limit { get; }
            public string? Search { get; }

            public GetAllBundlesQuery(string? sort = null, int page = 1, int limit = 10, string? search = null)
            {
                Sort = sort;
                Page = page;
                Limit = limit;
                Search = search;
            }
        }

        public class
            GetAllBundlesQueryHandler : IRequestHandler<GetAllBundlesQuery, APIResponse<IList<CreateBundleResponse>>>
        {
            private readonly AppDbContext _dbContext;
            private readonly ILogger<GetAllBundlesQueryHandler> _logger;

            public GetAllBundlesQueryHandler(AppDbContext dbContext, ILogger<GetAllBundlesQueryHandler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task<APIResponse<IList<CreateBundleResponse>>> Handle(GetAllBundlesQuery request,
                CancellationToken cancellationToken)
            {
                try
                {
                    // filter only not deleted bundles
                    var query = _dbContext.Bundles.Where(x => !x.IsDeleted);

                    // add search query if present
                    if (!string.IsNullOrWhiteSpace(request.Search))
                    {
                        var searchTerm = request.Search.ToLower();
                        query = query.Where(x => EF.Functions.Like(x.Title.ToLower(), $"%{searchTerm}%"));
                    }

                    // Apply sorting
                    query = request.Sort?.ToLower() switch
                    {
                        "price_asc" => query.OrderBy(x =>
                            x.BundlesProducts.Sum(bp => bp.Product.Price) - x.DiscountedPrice),
                        "price_desc" => query.OrderByDescending(x =>
                            x.BundlesProducts.Sum(bp => bp.Product.Price) - x.DiscountedPrice),
                        "recent" => query.OrderByDescending(x => x.CreatedAt),
                        _ => query.OrderByDescending(x => x.CreatedAt) // default sorting
                    };

                    var totalItems = await query.CountAsync(cancellationToken);

                    var bundlesList = await query
                        .Skip((request.Page - 1) * request.Limit)
                        .Take(request.Limit)
                        .Include(b => b.BundlesProducts)
                        .ThenInclude(bp => bp.Product)
                        .Select(b => new CreateBundleResponse
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Description = b.Description,
                            DiscountedPrice = b.DiscountedPrice,
                            TotalPrice = b.BundlesProducts.Sum(bp => bp.Product.Price),
                            Products = b.BundlesProducts.Select(bp => new ProductResponse
                            {
                                Id = bp.Product.Id,
                                Title = bp.Product.Title,
                                Description = bp.Product.Description,
                                Price = bp.Product.Price,
                                Discount = bp.Product.Discount,
                                CodeTitle = bp.Product.CodeTitle,
                                CurrentVersion = null
                            }).ToList()
                        })
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new APIResponse<IList<CreateBundleResponse>>(bundlesList, "Bundles retrieved successfully.",
                        request.Page, request.Limit, totalItems);
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
