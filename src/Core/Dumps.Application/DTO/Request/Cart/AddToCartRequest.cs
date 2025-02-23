using Dumps.Domain.Entities;

namespace Dumps.Application.DTO.Request.Cart
{
    public class AddToCartRequest
    {
        public Guid? ProductId { get; set; }
        public Guid? BundleId { get; set; }
    }
}
