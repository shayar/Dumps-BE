using Dumps.Application.DTO.Request.PromoCode;
using Dumps.Application.DTO.Response.PromoCode;
using Dumps.Application.Exceptions;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Dumps.Application.Command.Promo
{
    public class UpdatePromoCodeCommand : UpdatePromoCodeRequest, IRequest<APIResponse<PromoCodeResponse>>
    {
        public Guid Id { get; set; } // ID of the promo code to update
    }

    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, APIResponse<PromoCodeResponse>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdatePromoCodeCommandHandler> _logger;

        public UpdatePromoCodeCommandHandler(AppDbContext context, ILogger<UpdatePromoCodeCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<APIResponse<PromoCodeResponse>> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var promoCode = await _context.PromoCodes
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (promoCode == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, "Promo code not found.");
                }

                // Update only if new values are provided
                if (!string.IsNullOrEmpty(request.Code)) promoCode.Code = request.Code;
                if (request.DiscountType.HasValue) promoCode.DiscountType = request.DiscountType.Value;
                if (request.DiscountValue.HasValue) promoCode.DiscountValue = request.DiscountValue.Value;
                if (request.MaxDiscount.HasValue) promoCode.MaxDiscount = request.MaxDiscount.Value;
                if (request.IsActive.HasValue) promoCode.IsActive = request.IsActive.Value;
                if (request.ExpiryDate.HasValue) promoCode.ExpiryDate = request.ExpiryDate.Value;

                await _context.SaveChangesAsync(cancellationToken);

                var updatedResponse = new PromoCodeResponse
                {
                    Id = promoCode.Id,
                    Code = promoCode.Code,
                    DiscountType = promoCode.DiscountType,
                    DiscountValue = promoCode.DiscountValue,
                    MaxDiscount = promoCode.MaxDiscount,
                    IsActive = promoCode.IsActive,
                    ExpiryDate = promoCode.ExpiryDate
                };

                return new APIResponse<PromoCodeResponse>(updatedResponse, "Promo code updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating promo code.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while updating promo code.");
            }
        }
    }
}
