using Dumps.Domain.Common.Enums;

namespace Dumps.Application.DTO.Request.PromoCode
{
    public class CreatePromoCodeRequest
    {
        public string Code { get; set; } // Unique promo code
        public DiscountType DiscountType { get; set; } // Percentage or Flat
        public decimal DiscountValue { get; set; } // Discount amount or percentage
        public decimal MaxDiscount { get; set; } // Maximum discount allowed
        public bool IsActive { get; set; } = true; // Default active
        public DateTime ExpiryDate { get; set; } // Expiry date of the promo code
    }
}
