using Panier.Core.Redis.Connection.Abstract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Core.Redis.Connection.Concrete
{
    public class RedisConnection : IRedisConnection
    {
        #region Fields

        #region External Libraries
        private readonly Lazy<ConnectionMultiplexer> rediCon;
        #endregion

        #region Structs
        private string connectionString;
        #endregion

        #endregion

        #region Constructor
        public RedisConnection(string connectionStrings)
        {
            this.connectionString = connectionStrings;
            this.rediCon = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));

        }
        #endregion

        #region The Connecction
        public ConnectionMultiplexer Connection()
        {
            return this.rediCon.Value;
        }
        #endregion
    }

}
