using Newtonsoft.Json;
using Panier.Core.Redis.Connection.Abstract;
using Panier.Core.Redis.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Panier.Core.Redis.Repository.Concrete
{
    public class RedisRepository : IRedisRepository
    {

        private readonly IRedisConnection redisCon;


        #region Constructor
        public RedisRepository(IRedisConnection redisCon) => this.redisCon = redisCon;
        #endregion

        #region GET Object Async
        public async Task<T> GetObjectAsync<T>(string key)
        {
            try
            {
                var db = redisCon.Connection().GetDatabase();
                string data = await db.StringGetAsync(key);
                return data == null ? default(T) :
                    JsonConvert.DeserializeObject<T>(data);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region SET Object Async
        public async Task<bool> SetObjectAsync<T>(string key, T t)
        {
            bool result = false;
            try
            {
                var db = redisCon.Connection().GetDatabase();
                result = await db.StringSetAsync(key, JsonConvert.SerializeObject(t));
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SetObjectAsync<T>(string key, T t, DateTime expiration)
        {
            bool result = false;

            var timeSpanExpiration = expiration - DateTime.Now;
            timeSpanExpiration = TimeSpan.FromMilliseconds(timeSpanExpiration.TotalMilliseconds);
            if (timeSpanExpiration.TotalMilliseconds <= 0)
                return false;

            try
            {
                var db = redisCon.Connection().GetDatabase();
                result = await db.StringSetAsync(key, JsonConvert.SerializeObject(t), timeSpanExpiration);
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SetObjectAsync<T>(string key, T t, TimeSpan expiration)
        {
            bool result = false;

            try
            {
                var db = redisCon.Connection().GetDatabase();
                result = await db.StringSetAsync(key, JsonConvert.SerializeObject(t), expiration);
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region REMOVE Object Async
        public async Task<bool> RemoveObjectAsync(string key)
        {
            bool result = false;

            try
            {
                var db = redisCon.Connection().GetDatabase();
                result = await db.KeyDeleteAsync(key);
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        #endregion

      

    }
}
