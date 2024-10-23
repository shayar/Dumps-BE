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

                    // Upload the file using a third-party storage service
                    var pdfUrl = await _storageService.UploadFileAsync(request.PdfFile);

                    // Create the new product entity
                    var product = new Dumps.Domain.Entities.Products
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Price = request.Price,
                        Discount = request.Discount,
                        CurrentVersion = new ProductVersion
                        {
                            VersionNumber = 1,
                            PdfUrl = pdfUrl
                        },
                        ProductVersions = new List<ProductVersion>()
                    };

                    product.ProductVersions.Add(product.CurrentVersion);

                    // Save the product to the database
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync(cancellationToken);

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
