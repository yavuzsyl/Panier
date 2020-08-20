using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Panier.Contracts.V1.Requests;
using Panier.Contracts.V1.Responses;
using Panier.Core.LoggerService.Abstract;
using Panier.Core.Redis.Repository.Abstract;
using Panier.Domain.Entities;
using Panier.Domain.Repositories.Abstract;
using Panier.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Services.Concrete
{
    public class BasketItemtService : BaseService<BasketItem>, IBasketItemtService
    {
        public IAdvertisementService advertisementService;
        public IRedisRepository redisRepository;
        private readonly ILoggerManager logger;

        public BasketItemtService(IUnitOfWork unitOfWork,
            IBasketItemRepository repository,
            IAdvertisementService advertisementService,
            ILoggerManager logger) : base(unitOfWork, repository)
        {
            this.advertisementService = advertisementService;
            this.logger = logger;
        }



        public async Task<Response<BasketItem>> AddToBasket(BasketItem model, string currentUserId)
        {
            var advertisement = await advertisementService.FindEntityById(model.AdvertisementId);
            if (!advertisement.Success || !advertisement.Result.IsActive || advertisement.Result.IsDeleted)
                return new Response<BasketItem>(advertisement.Message);
            else if(advertisement.Result.UnitsInStock < model.Count)
                return new Response<BasketItem>("Not enough stock");


            //userId token
            var userBasketItem = (await repository.GetList(x => x.AppUserId == currentUserId && !x.IsDeleted && x.AdvertisementId == model.AdvertisementId)).FirstOrDefault();
            if (userBasketItem != null)
            {
                try
                {
                    userBasketItem.Count += model.Count;
                    userBasketItem.TotalPrice = userBasketItem.Count * advertisement.Result.Price;
                    repository.UpdateEntity(userBasketItem);
                    var result = await unitOfWork.CompleteAsync();
                    if (!result)
                        return new Response<BasketItem>("Couldnt update basketItem");//
                    logger.LogInfo($"Basket Item updated {JsonConvert.SerializeObject(userBasketItem)}");

                }
                catch (Exception ex )
                {
                    logger.LogError(ex.Message);
                    return new Response<BasketItem>(ex.Message);
                }

            }
            else
            {
                try
                {
                    userBasketItem = new BasketItem
                    {
                        Advertisement = advertisement.Result,
                        AppUserId = currentUserId, //token id,
                        Count = model.Count,
                        TotalPrice = model.Count * advertisement.Result.Price

                    };
                    await repository.AddEntity(userBasketItem);
                    var result = await unitOfWork.CompleteAsync();
                    if (!result)
                        return new Response<BasketItem>("Couldnt add basketItem");//
                    logger.LogInfo($"Basket Item added {JsonConvert.SerializeObject(userBasketItem)}");


                }
                catch (Exception ex)
                {

                    logger.LogError(ex.Message);
                    return new Response<BasketItem>(ex.Message);
                }
         

            }

            return new Response<BasketItem>(userBasketItem);

        }


    }
}
