using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Entities
{
    public class BasketItem : BaseEntity
    {
        public int Count { get; set; }
        public decimal TotalPrice { get; set; }

        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
