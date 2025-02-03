using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OjiSoftPortal.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace OjiSoftPortal.Data
{
    public class OjiSoftDataContext : IdentityDbContext<OjiUser>
    {
        public OjiSoftDataContext(DbContextOptions<OjiSoftDataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("nvarchar(256)");
            });
        }
    }
}
