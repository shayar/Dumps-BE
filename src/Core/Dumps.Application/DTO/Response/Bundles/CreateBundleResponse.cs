using Dumps.Application.DTO.Response.Products;

namespace Dumps.Application.DTO.Response.Bundles
{
    public class CreateBundleResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public IList<ProductResponse> Products { get; set; }
    }
}
