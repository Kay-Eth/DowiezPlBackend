using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public class DowiezPlDbContext : IdentityDbContext
    {
        public DbSet<City> Cities { get; set; }

        public DowiezPlDbContext(DbContextOptions<DowiezPlDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<City>()
                .HasIndex(c => new { c.CityName, c.CityDistrict })
                .IsUnique();
        }
    }
}