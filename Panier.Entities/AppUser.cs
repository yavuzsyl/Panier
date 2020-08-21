using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Entities
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Advertisements = new List<Advertisement>();
            BasketItems = new List<BasketItem>();
        }
        public List<Advertisement> Advertisements { get; set; }
        public List<BasketItem> BasketItems { get; set; }

    }
}
