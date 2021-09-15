using System;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public class DowiezPlDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<Demand> Demands { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Opinion> Opinions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Transport> Transports { get; set; }

        public DowiezPlDbContext(DbContextOptions<DowiezPlDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<City>()
                .HasIndex(c => new { c.CityName, c.CityDistrict })
                .IsUnique();

            builder.Entity<Opinion>()
                .HasIndex(new string[] {"IssuerId", "RatedId"})
                .IsUnique();
            
            builder.Entity<Opinion>()
                .HasCheckConstraint(
                    "CK_Issuer_cannot_rate_himself",
                    "IssuerId <> RatedId"
                );
            
            builder.Entity<Report>()
                .HasCheckConstraint(
                    "CK_Reporter_cannot_rate_himself",
                    "ReporterId <> ReportedId"
                );

            // builder.Entity<Opinion>()
            //     .(
            //         "CK_Issuer_Cannot_Rate_Himself",
            //         "[IssuerId] != [RatedId]"
            //     );
            
            // builder.Entity<AppUser>()
            //     .HasMany(u => u.IssuedOpinions)
            //     .WithOne(o => o.Issuer)
            //     .HasForeignKey(o => o.OpinionId);

            // builder.Entity<AppUser>()
            //     .HasMany(u => u.RatesRecieved)
            //     .WithOne(o => o.Rated)
            //     .HasForeignKey(o => o.OpinionId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}