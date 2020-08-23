using Panier.Business.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Abstract
{
    public interface IBaseService<T> where T : class
    {
        Task<Response<IEnumerable<T>>> ListAsync();
        Task<Response<T>> AddEntityAsync(T entity);
        Task<Response<T>> RemoveEntityAsync(int id);
        Task<Response<T>> UpdateEntityAsync(T entity, int id);
        Task<Response<T>> FindEntityById(int id);
    }
}
