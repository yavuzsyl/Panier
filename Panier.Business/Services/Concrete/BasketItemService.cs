
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Panier.Business.Contracts.V1.Responses;
using Panier.Business.Services.Abstract;
using Panier.Business.Services.Abstract.Mongo;
using Panier.Core.LoggerService.Abstract;
using Panier.Core.Redis.Repository.Abstract;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Entities;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Concrete
{
    public class BasketItemService : BaseService<BasketItem>, IBasketItemService
    {
        public IAdvertisementService advertisementService;
        public IRedisRepository redisRepository;
        private readonly ILoggerManager logger;
        private readonly IStatusMessageRepository _statusRepository;


        public BasketItemService(IUnitOfWork unitOfWork,
            IBasketItemRepository repository,
            IAdvertisementService advertisementService,
            IStatusMessageRepository _statusRepository,
            IRedisRepository redisRepository,
            ILoggerManager logger) : base(unitOfWork, repository)
        {
            this.advertisementService = advertisementService;
            this.logger = logger;
            this._statusRepository = _statusRepository;
            this.redisRepository = redisRepository;
        }


        public async Task<Response<BasketItem>> AddToBasket(BasketItem model, string currentUserId)
        {

            var advertisement = await advertisementService.FindEntityById(model.AdvertisementId);

            if (!advertisement.Success)
                return await ReturnUnkownStatusResponse<BasketItem>(advertisement.Message);
            else if (!advertisement.Result.IsActive || advertisement.Result.IsDeleted)
                return await ReturnStatusResponse<BasketItem>("NotActiveAdvertisement");
            else if (advertisement.Result.UnitsInStock < model.Count)
                return await ReturnStatusResponse<BasketItem>("NotEnoughStockAdvertisement");


            var userBasketItem = await repository.GetByExpression(x => x.AppUserId == currentUserId && !x.IsDeleted && x.AdvertisementId == model.AdvertisementId);
            if (userBasketItem != null)
            {
                try
                {
                    userBasketItem.Count += model.Count;
                    userBasketItem.TotalPrice = userBasketItem.Count * advertisement.Result.Price;
                    repository.UpdateEntity(userBasketItem);
                    var result = await unitOfWork.CompleteAsync();
                    if (!result)
                        return await ReturnStatusResponse<BasketItem>("CouldntUpdateBasketItem");
                    logger.LogInfo($"Basket item successfully updated: {JsonConvert.SerializeObject(userBasketItem)}");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return await ReturnUnkownStatusResponse<BasketItem>(ex.Message);
                }

            }
            else
            {
                try
                {
                    userBasketItem = new BasketItem
                    {
                        Advertisement = advertisement.Result,
                        AppUserId = currentUserId, 
                        Count = model.Count,
                        TotalPrice = model.Count * advertisement.Result.Price

                    };
                    await repository.AddEntity(userBasketItem);
                    var result = await unitOfWork.CompleteAsync();
                    if (!result)
                        return await ReturnStatusResponse<BasketItem>("CouldntInsertBasketItem");
                    logger.LogInfo($"Basket item successfully added: {JsonConvert.SerializeObject(userBasketItem)}");


                }
                catch (Exception ex)
                {

                    logger.LogError(ex.Message);
                    return await ReturnUnkownStatusResponse<BasketItem>(ex.Message);
                }


            }

            return new Response<BasketItem>(userBasketItem);

        }

        public async Task<Response<T>> ReturnStatusResponse<T>(string key) where T : BaseEntity
        {
            var statusModel = await redisRepository.GetObjectAsync<StatusMessage>(key);
            return new Response<T>(statusModel.statusMessage, statusModel.statusCode);
        }
        public async Task<Response<T>> ReturnUnkownStatusResponse<T>(string message) where T : BaseEntity
          => await Task.Run(() => new Response<T>(message));



    }
}
