using Panier.DataAccess.Data;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.DataAccess.Repositories.Concrete
{
    public class AdvertisementRepository :BaseRepository<Advertisement>, IAdvertisementRepository
    {
        public AdvertisementRepository(PanierContext context) : base(context)
        {

        }
    }
}
