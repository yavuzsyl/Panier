using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Panier.Domain.Entities;
using Panier.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Panier.Domain.Data
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
