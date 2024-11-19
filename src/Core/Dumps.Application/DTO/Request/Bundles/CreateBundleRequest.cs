namespace Dumps.Application.DTO.Request.Bundles
{
    public class CreateBundleRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountedPrice { get; set; }
        public IList<Guid> ProductIds { get; set; } // List of Product IDs to be included in the bundle
    }
}
