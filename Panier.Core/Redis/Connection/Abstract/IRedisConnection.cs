using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Core.Redis.Connection.Abstract
{
    public interface IRedisConnection
    {
        ConnectionMultiplexer Connection();
    }
}
