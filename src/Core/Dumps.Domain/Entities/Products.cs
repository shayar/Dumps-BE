using System.ComponentModel.DataAnnotations.Schema;

namespace Dumps.Domain.Entities
{
    public class Products : BaseEntity
    {
        public string Title { get; set; }
        public string CodeTitle { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public int Discount { get; set; } = 0;

        public Guid CurrentVersionId { get; set; }

        // Foreign key relationship to ProductVersion for the current version
        [ForeignKey("CurrentVersionId")]
        public virtual ProductVersion CurrentVersion { get; set; }

        // One-to-many relationship with ProductVersions
        [InverseProperty("Product")]
        public virtual ICollection<ProductVersion> ProductVersions { get; set; }
    }
}
