namespace Dumps.Domain.Entities
{
    public class Bundle : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountedPrice { get; set; }

        // Relationship
        public virtual ICollection<BundlesProducts> BundlesProducts { get; set; }
    }
}
