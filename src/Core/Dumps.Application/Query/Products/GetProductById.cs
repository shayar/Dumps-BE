using System.Linq.Expressions;
using System.Net;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Query.Products;

public class GetProductById
{
    public class GetProductByIdQuery : IRequest<APIResponse<ProductResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, APIResponse<ProductResponse>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetProductByIdHandler> _logger;

        public GetProductByIdHandler(AppDbContext dbContext, ILogger<GetProductByIdHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<APIResponse<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<Domain.Entities.Products, bool>> filter = x => !x.IsDeleted && x.Id == request.Id;

                var product = await _dbContext.Products
                    .Where(filter)
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                if (product == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, "Product not found");
                }

                var currentVersion = product.CurrentVersionId.HasValue
               ? await _dbContext.ProductVersions
                   .Where(pv => pv.Id == product.CurrentVersionId.Value)
                   .Select(pv => new ProductVersionResponse
                   {
                       Id = pv.Id,
                       VersionNumber = pv.VersionNumber,
                       PdfUrl = pv.PdfUrl
                   })
                   .FirstOrDefaultAsync(cancellationToken)
                   .ConfigureAwait(false)
               : null;

                return new APIResponse<ProductResponse>(new ProductResponse
                {
                    Id = product.Id,
                    Description = product.Description,
                    Price = product.Price,
                    Discount = product.Discount,
                    Title = product.Title,
                    CodeTitle = product.CodeTitle,
                    CurrentVersion = currentVersion
                }, "Product retrieved successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving the product.");
                throw new RestException(HttpStatusCode.InternalServerError, "Cannot Retrieve Product");
            }
        }
    }
}
