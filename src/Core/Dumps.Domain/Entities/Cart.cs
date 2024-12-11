using System.ComponentModel.DataAnnotations.Schema;

namespace Dumps.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Total price of the cart, calculated dynamically
        [NotMapped]
        public decimal TotalPrice => CartItems.Sum(item => item.ItemPrice);
    }
}
