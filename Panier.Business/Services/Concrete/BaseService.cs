
using Panier.Business.Contracts.V1.Responses;
using Panier.Business.Services.Abstract;
using Panier.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Concrete
{
    public class BaseService<T> : IBaseService<T> where T : class
    {

        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBaseRepository<T> repository;
        public BaseService(IUnitOfWork unitOfWork, IBaseRepository<T> repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }


        public async Task<Response<T>> AddEntityAsync(T entity)
        {
            try
            {
                await repository.AddEntity(entity);
                var result = await unitOfWork.CompleteAsync();
                if (!result)
                    return new Response<T>($"{typeof(T).Name} couldnt add due to unknown reasons");
                return new Response<T>(entity);
            }
            catch (Exception ex)
            {

                return new Response<T>($"{typeof(T).Name} couldnt add due to : {ex.Message}");
            }
        }

        public async Task<Response<T>> FindEntityById(int id)
        {
            try
            {
                var entity = await repository.GetById(id);
                if (entity == null)
                    return new Response<T>($"Not found any {typeof(T).Name} ");
                return new Response<T>(entity);
            }
            catch (Exception ex)
            {
                return new Response<T>($"{typeof(T).Name}  couldnt find due to : {ex.Message}");

            }
        }

        public async Task<Response<IEnumerable<T>>> ListAsync()
        {
            try
            {
                var entityList = await repository.GetList();
                if (entityList == null)
                    return new Response<IEnumerable<T>>($"Not found {typeof(T).Name} ");
                return new Response<IEnumerable<T>>(entityList);
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<T>>($"{typeof(T).Name} 's couldnt list due to : {ex.Message}");

            }
        }

        public async Task<Response<T>> RemoveEntityAsync(int id)
        {

            try
            {
                var entity = await repository.GetById(id);
                if (entity == null)
                    return new Response<T>($"Not found {typeof(T).Name} ");
                await repository.DeleteEntity(id);
                var result = await unitOfWork.CompleteAsync();
                if (!result)
                    return new Response<T>($"{typeof(T).Name} 's couldn't remove due to unknown reasons");

                return new Response<T>(entity);
            }
            catch (Exception ex)
            {
                return new Response<T>($"{typeof(T).Name} 's couldn't remove due to : {ex.Message}");
            }
        }

        public async Task<Response<T>> UpdateEntityAsync(T entity, int id)
        {
            try
            {
                repository.UpdateEntity(entity);
                var result = await unitOfWork.CompleteAsync();
                if (!result)
                    return new Response<T>($"{typeof(T).Name} 's couldn't update due to unknown reasons");
                return new Response<T>(entity);
            }
            catch (Exception ex)
            {
                return new Response<T>($"Update {typeof(T).Name} failed due to : {ex.Message}");

            }
        }
    }

}
