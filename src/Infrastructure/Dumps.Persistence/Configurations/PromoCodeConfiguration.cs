using Dumps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dumps.Persistence.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            // Use enum as an integer in the database
            builder.Property(p => p.DiscountType)
                   .HasConversion<int>();

            builder.Property(p => p.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.DiscountValue)
                   .IsRequired();

            builder.Property(p => p.MaxDiscount)
                   .IsRequired();

            builder.Property(p => p.IsActive)
                   .IsRequired();

            builder.Property(p => p.ExpiryDate)
                   .IsRequired();
        }
    }
}
