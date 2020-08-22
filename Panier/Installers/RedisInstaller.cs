using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Panier.Business.Options;
using Panier.Core.Redis.Connection.Abstract;
using Panier.Core.Redis.Connection.Concrete;
using Panier.Core.Redis.Repository.Abstract;
using Panier.Core.Redis.Repository.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Installers
{
    public class RedisInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            var redisSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisSettings);
            services.AddSingleton(redisSettings);
            services.AddSingleton<IRedisConnection, RedisConnection>(q => new RedisConnection(redisSettings.ConnectionString));
            services.AddTransient<IRedisRepository, RedisRepository>();

        }
    }
}
