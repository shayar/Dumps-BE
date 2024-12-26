using Dumps.Domain.Entities;

namespace Dumps.Application.DTO.Request.Cart
{
    public class AddToCartRequest
    {
        public IList<Guid> ProductIds { get; set; }
        public IList<Guid> BundleIds { get; set; }
    }
}
