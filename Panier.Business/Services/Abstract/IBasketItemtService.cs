
using Panier.Business.Contracts.V1.Responses;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Abstract
{
    public interface IBasketItemService : IBaseService<BasketItem>
    {
        Task<Response<BasketItem>> AddToBasket(BasketItem model,string currentUserId);
    }
}
