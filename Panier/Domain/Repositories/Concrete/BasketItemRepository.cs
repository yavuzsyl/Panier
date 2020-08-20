using Panier.Domain.Data;
using Panier.Domain.Entities;
using Panier.Domain.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Concrete
{
    public class BasketItemRepository : BaseRepository<BasketItem>, IBasketItemRepository
    {
        public BasketItemRepository(PanierContext context) : base(context)
        {

        }
    }
}
