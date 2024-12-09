using System.Net;
using Dumps.Application.DTO.Request.Products;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Products
{
    public class CreateProductCommand : CreateProductRequest, IRequest<CreateProductResponse>
    {
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;  // Storage service interface
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(AppDbContext context, IStorageService storageService, ILogger<CreateProductCommandHandler> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Start a database transaction
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // Validate file size or type if needed (optional)
                    if (request.PdfFile == null || request.PdfFile.Length == 0)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, "Invalid or missing PDF file.");
                    }

                    // Step 1: Create and save the product (with a temporary CurrentVersionId)
                    var product = new Dumps.Domain.Entities.Products
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Price = request.Price,
                        Discount = request.Discount,
                        CodeTitle = request.CodeTitle,
                        CurrentVersionId = Guid.Empty // Temporarily set to Guid.Empty
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Step 2: Create the ProductVersion and associate it with the product
                    var productVersion = new ProductVersion
                    {
                        ProductId = product.Id, // Set the ProductId to the newly created product's Id
                        VersionNumber = 1.0f, // Set an appropriate version number
                        PdfUrl = await _storageService.UploadFileAsync(request.PdfFile).ConfigureAwait(false),
                        FileName = request.PdfFile.FileName
                    };

                    _context.ProductVersions.Add(productVersion);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Step 3: Update the product with the correct CurrentVersionId
                    product.CurrentVersionId = productVersion.Id;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Commit the transaction if everything succeeds
                    await transaction.CommitAsync(cancellationToken);

                    return new CreateProductResponse
                    {
                        Succeeded = true,
                        ProductId = product.Id
                    };
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync(cancellationToken);

                    // Log the error
                    _logger.LogError(ex, "Error occurred while creating a new product.");

                    // Return a failed result with error messages
                    throw new RestException(HttpStatusCode.InternalServerError, "An error occurred during product creation.");
                }
            }
        }
    }
}
