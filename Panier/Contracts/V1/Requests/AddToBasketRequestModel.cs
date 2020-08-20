using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Contracts.V1.Requests
{
    public class AddToBasketRequestModel
    {
        public int AdvertisementId { get; set; }
        public int Count { get; set; }
    }
}
