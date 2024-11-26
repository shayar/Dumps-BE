using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.Command.Products;
using Dumps.Application.DTO.Request.Bundles;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Bundles
{
    public class DeleteBundleCommand : IRequest<APIResponse<object>>
    {
        public Guid Id { get; set; }
    }
    public class DeleteBundleCommandHandler : IRequestHandler<DeleteBundleCommand, APIResponse<object>>
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly ILogger<DeleteBundleCommandHandler> _logger;

        public DeleteBundleCommandHandler(AppDbContext context, IStorageService storageService, ILogger<DeleteBundleCommandHandler> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<APIResponse<object>> Handle(DeleteBundleCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the existing bundle
            var bundle = await _context.Bundles
                .Include(b => b.BundlesProducts)
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (bundle == null)
            {
                throw new RestException(HttpStatusCode.NotFound, "Bundle not found.");
            }

            // Delete the Bundle record from the database
            _context.Bundles.Remove(bundle);
            await _context.SaveChangesAsync(cancellationToken);

            return new APIResponse<object>(null, "Product deleted successfully.");
        }
    }
}
