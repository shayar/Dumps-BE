using System.ComponentModel.DataAnnotations.Schema;

namespace Dumps.Domain.Entities
{
    public class ProductVersion : BaseEntity
    {
        public Guid ProductId { get; set; }

        // Foreign key back to Product
        [ForeignKey("ProductId")]
        [InverseProperty("ProductVersions")]
        public virtual Products Product { get; set; }

        public float VersionNumber { get; set; }
        public string PdfUrl { get; set; }
        public string FileName { get; set; }
    }
}
