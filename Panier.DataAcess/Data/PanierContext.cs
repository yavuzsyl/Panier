using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Panier.DataAccess.Extensions;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace Panier.DataAccess.Data
{
    public class PanierContext : IdentityDbContext<AppUser>
    {
        public PanierContext(DbContextOptions<PanierContext> options) : base(options)
        {

        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<Product>().Property(x => x.Name).IsRequired(true);
            mb.Entity<BasketItem>().HasKey(x => new { x.AdvertisementId, x.AppUserId });

            mb.Seed();
        }
    }
}
