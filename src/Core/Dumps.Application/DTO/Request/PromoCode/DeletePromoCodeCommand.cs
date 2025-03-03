using Dumps.Application.Exceptions;
using Dumps.Domain.Common.Constants;
using Dumps.Persistence.DbContext;
using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Application.DTO.Request.PromoCode
{
    public class DeletePromoCodeCommand : IRequest<APIResponse<string>>
    {
        public Guid PromoCodeId { get; set; }
    }

    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, APIResponse<string>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeletePromoCodeCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeletePromoCodeCommandHandler(AppDbContext context, ILogger<DeletePromoCodeCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != SD.Role_Admin)
                {
                    throw new RestException(HttpStatusCode.Forbidden, "Only admins can delete promo codes.");
                }

                var promoCode = await _context.PromoCodes
                    .FirstOrDefaultAsync(p => p.Id == request.PromoCodeId, cancellationToken);

                if (promoCode == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, "Promo code not found.");
                }

                _context.PromoCodes.Remove(promoCode);
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse<string>("Promo code deleted successfully.", "Operation successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the promo code.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while deleting the promo code.");
            }
        }
    }
}
