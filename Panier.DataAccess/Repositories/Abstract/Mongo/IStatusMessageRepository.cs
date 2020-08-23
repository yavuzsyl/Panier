using Panier.Core.Mongo;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.DataAccess.Repositories.Abstract.Mongo
{
    public interface IStatusMessageRepository : IMongoBaseRepository<StatusMessage>
    {
    }
}
