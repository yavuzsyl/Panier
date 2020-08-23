using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Panier.Core.Redis.Repository.Abstract;
using Panier.Entities.Mongo;
using Panier.Helper;

namespace Panier
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //Setting required informations to mongo and redis when application starts //uygulama ayağa kalkarken mongoya ve redise setleme işlemi
            await DataInitializer.RedisCacheInitialzier(host);
            await host.RunAsync();

        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
