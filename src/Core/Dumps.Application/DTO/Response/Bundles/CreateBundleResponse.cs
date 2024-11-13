namespace Dumps.Application.DTO.Response.Bundles
{
    public class CreateBundleResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountedPrice { get; set; }
        public IList<Guid> ProductIds { get; set; }
    }
}
