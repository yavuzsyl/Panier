using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Panier.Core.LoggerManager.Concrete;
using Panier.Core.LoggerService.Abstract;
using Panier.Core.Redis.Connection.Abstract;
using Panier.Core.Redis.Connection.Concrete;
using Panier.Core.Redis.Repository.Abstract;
using AutoMapper;
using Panier.Core.Redis.Repository.Concrete;
using Panier.Installers;
using Panier.Business.Services.Abstract;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Business.Services.Concrete;
using Panier.DataAccess.Repositories.Concrete;
using Panier.Business.Options;
using Microsoft.Extensions.Options;
using Panier.Core.Mongo;
using Panier.Business.Services.Abstract.Mongo;
using Panier.Business.Services.Concrete.Mongo;

namespace Panier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var mongoDbSettings = new Core.Mongo.MongoDbSettings();
            Configuration.GetSection(nameof(Core.Mongo.MongoDbSettings)).Bind(mongoDbSettings);
            services.Configure<Core.Mongo.MongoDbSettings>(options => Configuration.GetSection("MongoDbSettings").Bind(options));

            services.AddSingleton(mongoDbSettings);
            services.AddSingleton<IMongoDBContext, MongoDBContext>();
            services.AddScoped<IStatusMessageRepository, StatusMessageRepository>();

            services.installServicesInAssembly(Configuration);

            services.AddControllers();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddAutoMapper(typeof(Startup));

            var redisSettings = new RedisCacheSettings();
            Configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisSettings);
            services.AddSingleton(redisSettings);
            services.AddSingleton<IRedisConnection, RedisConnection>(q => new RedisConnection(redisSettings.ConnectionString));

            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBasketItemRepository, BasketItemRepository>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
            services.AddScoped<IBasketItemService, BasketItemService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddTransient<IRedisRepository, RedisRepository>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(option => { option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
