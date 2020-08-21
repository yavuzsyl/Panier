
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
    public class BasketItemtService : BaseService<BasketItem>, IBasketItemtService
    {
        public IAdvertisementService advertisementService;
        public IRedisRepository redisRepository;
        private readonly ILoggerManager logger;
        private readonly IStatusMessageRepository _statusRepository;


        public BasketItemtService(IUnitOfWork unitOfWork,
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


        public async Task insert()
        {
           await _statusRepository.CreateMany(
                new List<StatusMessage>{
                new StatusMessage{
                    statusCode = 1,statusMessage = "This advertisement is not active",statusName = "NotActiveAdvertisement" },
                new StatusMessage{
                    statusCode = 2,statusMessage = "Not enough stock for this advertisement",statusName = "NotEnoughStockAdvertisement" },
                new StatusMessage{
                    statusCode = 3,statusMessage = "Couldnt update basketItem due to unkown resons",statusName = "CouldntUpdateBasketItem" },
                new StatusMessage{
                    statusCode = 4,statusMessage = "Couldnt insert new basketItem due to unkown resons ",statusName = "CouldntInsertBasketItem" },
                });

            var ms = await _statusRepository.Get();
            foreach (var item in ms)
            {
                await redisRepository.RemoveObjectAsync(item.statusName);
                var res = await redisRepository.SetObjectAsync<StatusMessage>(item.statusName,item);
            }
           
        }


        public async Task<Response<BasketItem>> AddToBasket(BasketItem model, string currentUserId)
        {

            await insert();
            var advertisement = await advertisementService.FindEntityById(model.AdvertisementId);

            if (!advertisement.Success)
                return new Response<BasketItem>(advertisement.Message);
            else if (!advertisement.Result.IsActive || advertisement.Result.IsDeleted)
                return new Response<BasketItem>((await redisRepository.GetObjectAsync<StatusMessage>("NotActiveAdvertisement")).statusMessage);
            else if (advertisement.Result.UnitsInStock < model.Count)
                return new Response<BasketItem>((await redisRepository.GetObjectAsync<StatusMessage>("NotEnoughStockAdvertisement")).statusMessage);


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
                        return new Response<BasketItem>((await redisRepository.GetObjectAsync<StatusMessage>("CouldntUpdateBasketItem")).statusMessage);//
                    logger.LogInfo($"Basket Item updated {JsonConvert.SerializeObject(userBasketItem)}");

                }
                catch (Exception ex)
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
                        return new Response<BasketItem>((await redisRepository.GetObjectAsync<StatusMessage>("CouldntInsertBasketItem")).statusMessage);//
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
