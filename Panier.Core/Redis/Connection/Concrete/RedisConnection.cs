using Panier.Core.Redis.Connection.Abstract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Core.Redis.Connection.Concrete
{
    public class RedisConnection : IRedisConnection
    {

        private readonly Lazy<ConnectionMultiplexer> rediCon;

        private string connectionString;

        public RedisConnection(string connectionStrings)
        {
            this.connectionString = connectionStrings;
            this.rediCon = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));

        }

        public ConnectionMultiplexer Connection()
        {
            return this.rediCon.Value;
        }
    }

}
