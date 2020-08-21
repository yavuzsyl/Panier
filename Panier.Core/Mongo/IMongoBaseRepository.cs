using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Panier.Core.Mongo
{
    public interface IMongoBaseRepository<TEntity> where TEntity : class
    {
        Task Create(TEntity obj);
        Task CreateMany(List<TEntity> obj);
        void Update(string id, TEntity obj);
        void Delete(string id);
        Task<TEntity> Get(string id);
        Task<IEnumerable<TEntity>> Get();
    }
}
