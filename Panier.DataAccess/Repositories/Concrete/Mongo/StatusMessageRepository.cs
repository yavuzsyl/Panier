using Panier.Core.Mongo;
using Panier.DataAccess.Repositories.Abstract.Mongo;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.DataAccess.Repositories.Concrete.Mongo
{
    public class StatusMessageRepository : MongoBaseRepository<StatusMessage>, IStatusMessageRepository
    {
        public StatusMessageRepository(IMongoDBContext context) : base(context)
        {
        }
    }
}
