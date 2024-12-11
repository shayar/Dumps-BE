using System.ComponentModel.DataAnnotations.Schema;

namespace Dumps.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? BundleId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        [ForeignKey("ProductId")]
        public Products Product { get; set; }

        [ForeignKey("BundleId")]
        public Bundle Bundle { get; set; }

        [NotMapped]
        public decimal ItemPrice => Product?.Price ?? Bundle?.DiscountedPrice ?? 0;
    }
}
