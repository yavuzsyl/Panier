using Panier.Business.Services.Abstract.Mongo;
using Panier.Core.Mongo;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Business.Services.Concrete.Mongo
{
    public class StatusMessageRepository : MongoBaseRepository<StatusMessage>, IStatusMessageRepository
    {
        public StatusMessageRepository(IMongoDBContext context) : base(context)
        {
        }
    }
}
