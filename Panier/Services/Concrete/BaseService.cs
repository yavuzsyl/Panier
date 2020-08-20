using Panier.Contracts.V1.Responses;
using Panier.Domain.Repositories.Abstract;
using Panier.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Services.Concrete
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
                await unitOfWork.CompleteAsync();
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

        public async Task<Response<IQueryable<T>>> ListAsync()
        {
            try
            {
                var entityList = await repository.GetList();
                if (entityList == null)
                    return new Response<IQueryable<T>>($"Not found {typeof(T).Name} ");
                return new Response<IQueryable<T>>(entityList);
            }
            catch (Exception ex)
            {
                return new Response<IQueryable<T>>($"{typeof(T).Name} 's couldnt list due to : {ex.Message}");

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
                await unitOfWork.CompleteAsync();
                return new Response<T>(entity);
            }
            catch (Exception ex)
            {
                return new Response<T>($"{typeof(T).Name} 's couldnt list due to : {ex.Message}");
            }
        }

        public async Task<Response<T>> UpdateEntityAsync(T entity, int id)
        {
            try
            {
                repository.UpdateEntity(entity);
                await unitOfWork.CompleteAsync();
                return new Response<T>(entity);
            }
            catch (Exception ex)
            {
                return new Response<T>($"Update {typeof(T).Name}  failed due to : {ex.Message}");

            }
        }
    }

}
