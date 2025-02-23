using System.Net;
using System.Security.Claims;
using Dumps.Application.DTO.Response.PromoCode;
using Dumps.Application.Exceptions;
using Dumps.Domain.Common.Constants;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dumps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Application.Query.PromoCode
{
    public class GetPromoCodesQuery : IRequest<APIResponse<List<PromoCodeResponse>>>
    {
        public bool? ShowAll { get; set; } // If null or false, only active promo codes will be fetched
    }

    public class GetPromoCodesQueryHandler : IRequestHandler<GetPromoCodesQuery, APIResponse<List<PromoCodeResponse>>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GetPromoCodesQueryHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPromoCodesQueryHandler(AppDbContext context, ILogger<GetPromoCodesQueryHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<List<PromoCodeResponse>>> Handle(GetPromoCodesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != SD.Role_Admin)
                {
                    throw new RestException(HttpStatusCode.Forbidden, "Only admins can view promo codes.");
                }

                IQueryable<Dumps.Domain.Entities.PromoCode> query = _context.PromoCodes;

                if (request.ShowAll.HasValue)
                {
                    if (!request.ShowAll.Value) // Show only inactive promo codes
                    {
                        query = query.Where(p => !p.IsActive);
                    }
                    // If ShowAll = true, fetch both active + inactive promo codes (no filter applied)
                }
                else
                {
                    query = query.Where(p => p.IsActive); // Default to only active promo codes
                }

                var promoCodes = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PromoCodeResponse
                    {
                        Id = p.Id,
                        Code = p.Code,
                        DiscountType = p.DiscountType,
                        DiscountValue = p.DiscountValue,
                        MaxDiscount = p.MaxDiscount,
                        IsActive = p.IsActive,
                        ExpiryDate = p.ExpiryDate
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse<List<PromoCodeResponse>>(promoCodes, "Promo codes retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving promo codes.");
                throw new RestException(HttpStatusCode.InternalServerError, "An error occurred while retrieving promo codes.");
            }
        }
    }
}
