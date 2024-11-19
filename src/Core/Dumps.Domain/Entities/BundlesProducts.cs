namespace Dumps.Domain.Entities
{
    public class BundlesProducts
    {
        public Guid BundleId { get; set; }
        public Guid ProductId { get; set; }

        public virtual Bundle Bundle { get; set; }
        public virtual Products Product { get; set; }
    }
}
