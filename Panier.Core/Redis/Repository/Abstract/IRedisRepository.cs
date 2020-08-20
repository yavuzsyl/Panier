using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Panier.Core.Redis.Repository.Abstract
{
    public interface IRedisRepository
    {
        Task<T> GetObjectAsync<T>(string key);
        Task<bool> SetObjectAsync<T>(string key, T t);
        Task<bool> SetObjectAsync<T>(string key, T t, DateTime expiration);
        Task<bool> SetObjectAsync<T>(string key, T t, TimeSpan expiration);
        Task<bool> RemoveObjectAsync(string key);

    }
}
