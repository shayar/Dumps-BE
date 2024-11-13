using System.Net;
using System.Reflection.Metadata;
using Dumps.Application.APIResponse;
using Dumps.Application.DTO.Request.Bundles;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.Exceptions;
using Dumps.Domain.Common.Constants;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Bundles
{
    public class CreateBundleCommand : CreateBundleRequest, IRequest<APIResponse<CreateBundleResponse>>
    {
    }

    public class CreateBundleCommandHandler : IRequestHandler<CreateBundleCommand, APIResponse<CreateBundleResponse>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateBundleCommandHandler> _logger;

        public CreateBundleCommandHandler(AppDbContext context, ILogger<CreateBundleCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<APIResponse<CreateBundleResponse>> Handle(CreateBundleCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Check if all product IDs are valid
                var products = await _context.Products
                    .Where(p => request.ProductIds.Contains(p.Id) && !p.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (products.Count != request.ProductIds.Count)
                {
                    throw new RestException(HttpStatusCode.BadRequest, "One or more product IDs are invalid.");
                }

                // Create a new bundle entity
                var bundle = new Bundle
                {
                    Title = request.Title,
                    Description = request.Description,
                    DiscountedPrice = request.DiscountedPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = SD.Role_Admin
                };

                _context.Bundles.Add(bundle);
                await _context.SaveChangesAsync(cancellationToken);

                // Create bundle-product associations
                var bundleProducts = request.ProductIds
                    .Select(productId => new BundlesProducts
                    {
                        BundleId = bundle.Id,
                        ProductId = productId
                    })
                    .ToList();

                _context.BundlesProducts.AddRange(bundleProducts);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Prepare response
                var response = new CreateBundleResponse
                {
                    Id = bundle.Id,
                    Title = bundle.Title,
                    Description = bundle.Description,
                    DiscountedPrice = bundle.DiscountedPrice,
                    ProductIds = request.ProductIds,
                };

                _logger.LogInformation("Bundle created successfully with ID: {BundleId}", bundle.Id);
                return new APIResponse<CreateBundleResponse>(response, "Bundle created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(ex, "Error occurred while creating a new bundle.");

                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred during bundle creation.");
            }
        }
    }
}
