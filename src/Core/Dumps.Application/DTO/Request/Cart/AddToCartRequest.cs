namespace Dumps.Application.DTO.Request.Cart
{
    public class AddToCartRequest
    {
        public IList<CartItemRequest> Items { get; set; }

        public class CartItemRequest
        {
            public Guid? ProductId { get; set; }
            public Guid? BundleId { get; set; }
        }
    }
}
