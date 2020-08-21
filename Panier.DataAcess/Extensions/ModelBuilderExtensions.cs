using Microsoft.EntityFrameworkCore;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.DataAccess.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder mb)
        {
            mb.Entity<Product>().HasData(new Product
            {
                Id = 1,
                Name = "Flower"

            });
            mb.Entity<AppUser>().HasData(new AppUser
            {

                Id = "9bee167a-34f4-4a56-9de9-07c332b6defd",
                Email = "flower@flower.com",
                UserName = "flower@flower.com"

            });
            mb.Entity<Advertisement>().HasData(new Advertisement
            {
                Id = 1,
                Price = 1.0m,
                ProductId = 1,
                UnitsInStock = 100,
                SellerId = "9bee167a-34f4-4a56-9de9-07c332b6defd"

            });
        }
    }
}
