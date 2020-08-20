﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Abstract
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IQueryable<T>> GetList(Expression<Func<T, bool>> expression = null);
        Task<int> GetCount(Expression<Func<T, bool>> expression = null);
        Task AddEntity(T entity);
        void UpdateEntity(T entity);
        Task DeleteEntity(int id);
    }
}
