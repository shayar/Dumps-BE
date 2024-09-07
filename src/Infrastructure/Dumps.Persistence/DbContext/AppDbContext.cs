using Dumps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Persistence.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ContactUs> ContactUsMessages { get; set; }
    }
}
