using Panier.Contracts.V1.Requests;
using Panier.Contracts.V1.Responses;
using Panier.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Services.Abstract
{
    public interface IBasketItemtService : IBaseService<BasketItem>
    {
        Task<Response<BasketItem>> AddToBasket(BasketItem model,string currentUserId);
    }
}
