using Dumps.Domain.Common.Enums;

namespace Dumps.Domain.Entities
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; } // "Percentage" or "Flat"
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscount { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
