using System.Linq.Expressions;
using System.Net;
using Dumps.Application.APIResponse;
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
    }

    public class GetAllProductsQueryHandler(AppDbContext dbContext, ILogger<GetAllProductsQueryHandler> logger)
        : IRequestHandler<GetAllProductsQuery, APIResponse<IList<ProductResponse>>>
    {
        public async Task<APIResponse<IList<ProductResponse>>> Handle(GetAllProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<Domain.Entities.Products, bool>> filter = x => !x.IsDeleted;

                var productsList = await dbContext.Products
                    .Where(filter)
                    .OrderByDescending(res => res.CreatedAt)
                    .Select(res => new ProductResponse
                    {
                        Id = res.Id,
                        Description = res.Description,
                        Price = res.Price,
                        Discount = res.Discount,
                        Title = res.Title,
                        CodeTitle = res.CodeTitle,
                    })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new APIResponse<IList<ProductResponse>>(productsList, "Products retrieved successfully.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while creating a new product.");
                throw new RestException(HttpStatusCode.InternalServerError, "Cannot Retrieve Products");
            }
        }
    }
}
