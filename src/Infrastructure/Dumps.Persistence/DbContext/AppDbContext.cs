using Dumps.Domain.Entities;
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

        public DbSet<ContactUs> ContactUsMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-many relationship between Product and ProductVersion
            modelBuilder.Entity<Products>()
                .HasMany(p => p.ProductVersions)
                .WithOne(pv => pv.Product)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade on delete
        }
    }
}
