using System.Net;
using Dumps.Application.APIResponse;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Products
{
    public class DeleteProductCommand : IRequest<APIResponse<object>>
    {
        public Guid ProductId { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, APIResponse<object>>
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(AppDbContext context, IStorageService storageService, ILogger<DeleteProductCommandHandler> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<APIResponse<object>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the product from the database
            var product = await _context.Products
                .Include(p => p.CurrentVersion)  // Assuming `CurrentVersion` has the file URL
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
                .ConfigureAwait(false);

            if (product == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Product not found.");
            }

            // Attempt to delete the associated file in Cloudinary
            if (product.CurrentVersion != null && !string.IsNullOrEmpty(product.CurrentVersion.PdfUrl))
            {
                try
                {
                    await _storageService.DeleteFileAsync(product.CurrentVersion.PdfUrl).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete file from storage.");
                    throw new RestException(HttpStatusCode.InternalServerError, "Failed to delete file from storage.");
                }
            }

            // Delete the product record from the database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return new APIResponse<object>(null, "Product deleted successfully.");
        }
    }
}
