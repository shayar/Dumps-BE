using System.Net;
using Dumps.Application.APIResponse;
using Dumps.Application.DTO.Request.Products;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Products
{
    public class UpdateProductCommand : CreateProductRequest, IRequest<APIResponse<Guid>>
    {
        public Guid ProductId { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, APIResponse<Guid>>
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(AppDbContext context, IStorageService storageService, ILogger<UpdateProductCommandHandler> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<APIResponse<Guid>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // Retrieve the product from the database
                    var product = await _context.Products
                    .Include(p => p.ProductVersions)
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
                    .ConfigureAwait(false);

                    if (product == null)
                    {
                        throw new RestException(HttpStatusCode.NotFound, "Product not found.");
                    }
                    // Update product details
                    product.Title = request.Title;
                    product.Description = request.Description;
                    product.Price = request.Price;
                    product.Discount = request.Discount;
                    product.CodeTitle = request.CodeTitle;

                    if (request.PdfFile != null && request.PdfFile.Length > 0)
                    {
                        // Fetch the latest version to calculate the next version number
                        var latestVersion = product.ProductVersions
                            .OrderByDescending(v => v.VersionNumber)
                            .FirstOrDefault();

                        float currentVersionNumber = latestVersion?.VersionNumber ?? 1.0f;

                        // Calculate the next version number
                        // Here the version number goes from 1.0, 1.1...1.9 and 2.0 ..2.9 like this
                        float newVersionNumber;
                        if (Math.Abs((currentVersionNumber % 1) - 0.9f) < 0.0001f)
                        {
                            newVersionNumber = (float)Math.Floor(currentVersionNumber) + 1; // Increment for version before decimal
                        }
                        else
                        {
                            newVersionNumber = currentVersionNumber + 0.1f; // Increment for version after decimal
                        }
                        // Upload new PDF and create a new product version
                        var newPdfUrl = await _storageService.UploadFileAsync(request.PdfFile).ConfigureAwait(false);
                        var newProductVersion = new ProductVersion
                        {
                            ProductId = product.Id,
                            VersionNumber = newVersionNumber,
                            PdfUrl = newPdfUrl
                        };

                        _context.ProductVersions.Add(newProductVersion);

                        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                        // Update the current version ID of the product
                        product.CurrentVersionId = newProductVersion.Id;
                    }
                    // Update product record
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken);

                    return new APIResponse<Guid>(product.Id, "Product updated successfully.");
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error occurred while updating the product.");
                    throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while updating the product.");
                }
            } 
        }
        
    }
 }


