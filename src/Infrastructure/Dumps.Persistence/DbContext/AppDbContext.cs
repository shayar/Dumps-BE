using Dumps.Domain.Entities;
using Dumps.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Persistence.DbContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductVersion> ProductVersions { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<BundlesProducts> BundlesProducts { get; set; }

        public DbSet<ContactUs> ContactUsMessages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configuration for PromoCode
            modelBuilder.ApplyConfiguration(new PromoCodeConfiguration());

            // One-to-many relationship between Product and ProductVersion
            modelBuilder.Entity<Products>()
                .HasMany(p => p.ProductVersions)
                .WithOne(pv => pv.Product)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade on delete

            // Define composite primary key for BundlesProducts
            modelBuilder.Entity<BundlesProducts>()
                .HasKey(bp => new { bp.BundleId, bp.ProductId });

            // Many-to-many relationship between Bundles and Products
            modelBuilder.Entity<BundlesProducts>()
                .HasOne(bp => bp.Bundle)
                .WithMany(b => b.BundlesProducts)
                .HasForeignKey(bp => bp.BundleId);

            modelBuilder.Entity<BundlesProducts>()
                .HasOne(bp => bp.Product)
                .WithMany(p => p.BundlesProducts)
                .HasForeignKey(bp => bp.ProductId);

            modelBuilder.Entity<Cart>()
        .HasOne(c => c.User)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
