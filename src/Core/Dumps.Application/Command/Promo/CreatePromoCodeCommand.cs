using System.Net;
using System.Security.Claims;
using Dumps.Application.DTO.Request.PromoCode;
using Dumps.Application.Exceptions;
using Dumps.Domain.Common.Constants;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Promo
{
    public class CreatePromoCodeCommand : CreatePromoCodeRequest, IRequest<APIResponse<Guid>>
    {
    }

    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, APIResponse<Guid>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreatePromoCodeCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreatePromoCodeCommandHandler(AppDbContext context, ILogger<CreatePromoCodeCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<Guid>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != SD.Role_Admin)
                {
                    throw new RestException(HttpStatusCode.Forbidden, "Only admins can create promo codes.");
                }

                // Ensure the promo code is unique
                var existingPromo = await _context.PromoCodes
                    .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

                if (existingPromo != null)
                {
                    throw new RestException(HttpStatusCode.Conflict, "Promo code already exists.");
                }

                var promoCode = new PromoCode
                {
                    Code = request.Code,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    MaxDiscount = request.MaxDiscount,
                    IsActive = request.IsActive,
                    ExpiryDate = request.ExpiryDate
                };

                _context.PromoCodes.Add(promoCode);
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse<Guid>(promoCode.Id, "Promo code created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a promo code.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while creating the promo code.");
            }
        }
    }
}
