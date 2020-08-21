using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            Advertisements = new List<Advertisement>();
        }
        public string Name { get; set; }

        public List<Advertisement> Advertisements { get; set; }
    }
}
