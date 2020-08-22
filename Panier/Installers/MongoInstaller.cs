﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Panier.Business.Services.Abstract.Mongo;
using Panier.Business.Services.Concrete.Mongo;
using Panier.Core.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Installers
{
    public class MongoInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = new Core.Mongo.MongoDbSettings();
            configuration.GetSection(nameof(Core.Mongo.MongoDbSettings)).Bind(mongoDbSettings);
            services.Configure<Core.Mongo.MongoDbSettings>(options => configuration.GetSection("MongoDbSettings").Bind(options));

            services.AddSingleton(mongoDbSettings);
            services.AddSingleton<IMongoDBContext, MongoDBContext>();
            services.AddScoped<IStatusMessageRepository, StatusMessageRepository>();
        }
    }
}
