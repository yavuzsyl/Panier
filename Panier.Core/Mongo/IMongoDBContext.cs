using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Core.Mongo
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
