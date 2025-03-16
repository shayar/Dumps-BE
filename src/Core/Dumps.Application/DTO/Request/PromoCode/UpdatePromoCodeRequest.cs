using Dumps.Domain.Common.Enums;

namespace Dumps.Application.DTO.Request.PromoCode
{
    public class UpdatePromoCodeRequest
    {
        public string? Code { get; set; }
        public DiscountType? DiscountType { get; set; } // Enum: Percentage or Flat
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
