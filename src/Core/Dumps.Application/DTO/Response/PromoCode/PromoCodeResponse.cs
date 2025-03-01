using Dumps.Domain.Common.Enums;

namespace Dumps.Application.DTO.Response.PromoCode
{
    public class PromoCodeResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscount { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
