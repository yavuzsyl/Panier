using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Domain.Entities
{
    public class Advertisement : BaseEntity
    {
        public Advertisement()
        {
            IsActive = true;
        }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }


        [ForeignKey(nameof(AppUser))]
        public string SellerId { get; set; }
        public AppUser Seller { get; set; }
    }
}
