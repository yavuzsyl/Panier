using Microsoft.EntityFrameworkCore;
using Panier.Domain.Data;
using Panier.Domain.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Concrete
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly PanierContext db;
        private readonly DbSet<T> entities;
        public BaseRepository(PanierContext db)
        {
            this.db = db;
            entities = db.Set<T>();
        }

        public async Task AddEntity(T entity)
        {
            await entities.AddAsync(entity);
        }

        public async Task DeleteEntity(int id)
        {
            entities.Remove(await GetById(id));
        }

        public async Task<T> GetById(int id)
        {
            return await entities.FindAsync(id);
        }

        public async Task<int> GetCount(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
                return await entities.CountAsync(expression);
            return await entities.CountAsync();
        }

        public async Task<IQueryable<T>> GetList(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
                return await Task.FromResult(entities.Where(expression));
            return await Task.FromResult(entities);
        }

        public void UpdateEntity(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }

}
