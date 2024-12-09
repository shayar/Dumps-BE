using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Products;

public class GetAllProducts
{
    public class GetAllProductsQuery : IRequest<APIResponse<IList<ProductResponse>>>
    {
        public string? Sort { get; }
        public int Page { get; }
        public int Limit { get; }
        public string? Search { get; }

        public GetAllProductsQuery(string? sort = null, int page = 1, int limit = 10, string? search = null)
        {
            Sort = sort;
            Page = page;
            Limit = limit;
            Search = search;
        }
    }

    public class GetAllProductsQueryHandler(AppDbContext dbContext, ILogger<GetAllProductsQueryHandler> logger)
        : IRequestHandler<GetAllProductsQuery, APIResponse<IList<ProductResponse>>>
    {
        public async Task<APIResponse<IList<ProductResponse>>> Handle(GetAllProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = dbContext.Products.Where(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var searchTerm = request.Search.ToLower();
                    query = query.Where(x => EF.Functions.Like(x.Title.ToLower(), $"%{searchTerm}%"));
                }

                // Apply sorting
                query = request.Sort?.ToLower() switch
                {
                    "price_asc" => query.OrderBy(x => x.Price),
                    "price_desc" => query.OrderByDescending(x => x.Price),
                    "recent" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderByDescending(x => x.CreatedAt) // default sorting
                };

                var totalItems = await query.CountAsync(cancellationToken);

                var productsList = await query
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .Select(res => new ProductResponse
                    {
                        Id = res.Id,
                        Description = res.Description,
                        Price = res.Price,
                        Discount = res.Discount,
                        Title = res.Title,
                        CodeTitle = res.CodeTitle,
                        CurrentVersion = res.CurrentVersionId.HasValue
                            ? dbContext.ProductVersions
                                .Where(pv => pv.Id == res.CurrentVersionId.Value)
                                .Select(pv => new ProductVersionResponse
                                {
                                    Id = pv.Id,
                                    VersionNumber = pv.VersionNumber,
                                    PdfUrl = pv.PdfUrl,
                                    FileName = pv.FileName
                                })
                                .FirstOrDefault()
                            : null
                    })
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return new APIResponse<IList<ProductResponse>>(
                    productsList,
                    "Products retrieved successfully.",
                    request.Page,
                    request.Limit,
                    totalItems
                );
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while retrieving products.");
                throw new RestException(HttpStatusCode.InternalServerError, "Cannot Retrieve Products");
            }
        }
    }
}
