using System.Net;
using Dumps.Application.APIResponse;
using Dumps.Application.DTO.Request.Bundles;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Bundles
{
    public class UpdateBundleCommand : UpdateBundleRequest, IRequest<APIResponse<Guid>>
    {
    }

    public class UpdateBundleCommandHandler : IRequestHandler<UpdateBundleCommand, APIResponse<Guid>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdateBundleCommandHandler> _logger;

        public UpdateBundleCommandHandler(AppDbContext context, ILogger<UpdateBundleCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<APIResponse<Guid>> Handle(UpdateBundleCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Retrieve the existing bundle
                var bundle = await _context.Bundles
                    .Include(b => b.BundlesProducts)
                    .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

                if (bundle == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, "Bundle not found.");
                }

                // Update fields only if they are provided
                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    bundle.Title = request.Title;
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    bundle.Description = request.Description;
                }

                if (request.DiscountedPrice.HasValue)
                {
                    bundle.DiscountedPrice = request.DiscountedPrice.Value;
                }

                // Update the product relationships if ProductIds are provided
                if (request.ProductIds != null && request.ProductIds.Any())
                {
                    // Check if all ProductIds exist in the Products table
                    var existingProductIds = await _context.Products
                        .Where(p => request.ProductIds.Contains(p.Id) && !p.IsDeleted)
                        .Select(p => p.Id)
                        .ToListAsync(cancellationToken);

                    if (existingProductIds.Count != request.ProductIds.Count)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, "One or more ProductIds are invalid.");
                    }

                    // Clear existing bundle-product relationships
                    _context.BundlesProducts.RemoveRange(bundle.BundlesProducts);

                    // Add new bundle-product relationships
                    foreach (var productId in request.ProductIds)
                    {
                        var bundlesProducts = new BundlesProducts
                        {
                            BundleId = bundle.Id,
                            ProductId = productId
                        };
                        _context.BundlesProducts.Add(bundlesProducts);
                    }
                }

                // Update audit fields
                bundle.UpdateAuditFields(request.UpdatedBy);

                _context.Bundles.Update(bundle);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new APIResponse<Guid>(bundle.Id, "Bundle updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while updating the bundle.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred during bundle update.");
            }
        }
    }
}
