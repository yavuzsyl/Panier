using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Panier.Core.Redis.Repository.Abstract;
using Panier.DataAccess.Repositories.Abstract.Mongo;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Helper
{

    public static class DataInitializer
    {
        /// <summary>
        /// Insert status message informations to mongo db and cache to redis / status mesaj bilgilierini mongoya kaydetme ve redis'e setleme
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task RedisCacheInitialzier(IHost host)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var mongoStatusRepo = serviceScope.ServiceProvider.GetRequiredService<IStatusMessageRepository>();
                var ms = await mongoStatusRepo.Get();

                if (!ms.Any())
                {
                    var redisRepository = serviceScope.ServiceProvider.GetRequiredService<IRedisRepository>();
                    var statusList = new List<StatusMessage>{
                        new StatusMessage{
                            statusCode = 1000,statusMessage = "This advertisement is not active",statusName = "NotActiveAdvertisement" },
                        new StatusMessage{
                            statusCode = 1001,statusMessage = "Not enough stock for this advertisement",statusName = "NotEnoughStockAdvertisement" },
                        new StatusMessage{
                            statusCode = 1002,statusMessage = @"Couldn't update basketItem due to unkown resons",statusName = "CouldntUpdateBasketItem" },
                        new StatusMessage{
                            statusCode = 1003,statusMessage = @"Couldn't insert new basketItem due to unkown resons ",statusName = "CouldntInsertBasketItem" },
                      };
                    try
                    {
                        await mongoStatusRepo.CreateMany(statusList);
                        foreach (var item in statusList)
                        {
                            await redisRepository.RemoveObjectAsync(item.statusName);
                            var res = await redisRepository.SetObjectAsync<StatusMessage>(item.statusName, item);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }


                }

            }
        }

    }
}
