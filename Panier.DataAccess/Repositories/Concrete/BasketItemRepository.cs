using Panier.DataAccess.Data;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.DataAccess.Repositories.Concrete
{
    public class BasketItemRepository : BaseRepository<BasketItem>, IBasketItemRepository
    {
        public BasketItemRepository(PanierContext context) : base(context)
        {

        }
    }
}