namespace Dumps.Application.DTO.Request.Bundles
{
    public class UpdateBundleRequest
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public IList<Guid>? ProductIds { get; set; }
        public string UpdatedBy { get; set; }
    }
}
