using Panier.Domain.Data;
using Panier.Domain.Entities;
using Panier.Domain.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Concrete
{
    public class AdvertisementRepository :BaseRepository<Advertisement>, IAdvertisementRepository
    {
        public AdvertisementRepository(PanierContext context) : base(context)
        {

        }
    }
}
