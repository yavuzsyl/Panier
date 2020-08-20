using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using Panier.Core.LoggerManager.Concrete;
using Panier.Core.LoggerService.Abstract;
using Panier.Core.Redis.Connection.Abstract;
using Panier.Core.Redis.Connection.Concrete;
using Panier.Core.Redis.Repository.Abstract;
using Panier.Domain.Data;
using Panier.Domain.Entities;
using Panier.Domain.Repositories.Abstract;
using Panier.Domain.Repositories.Concrete;
using Panier.Services.Abstract;
using Panier.Services.Concrete;
using AutoMapper;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Panier.Options;
using Panier.Core.Redis.Repository.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Panier.Filters;
using FluentValidation.AspNetCore;
using Panier.Extension;

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
            services.AddControllers();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddAutoMapper(typeof(Startup));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();//validation filter for modelstate control

            }).AddFluentValidation(mvcConfiguration => mvcConfiguration
                .RegisterValidatorsFromAssemblyContaining<Startup>())//going to register AbstractValidator validators
             .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #region settings
            var jwtSettings = new JwtSettings();
            Configuration.Bind(key: nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);
            services.Configure<JwtSettings>(Configuration.GetSection("jwtsettings"));
            #endregion


            services.AddDbContextPool<PanierContext>(options =>
            options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<AppUser>(opts =>  {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 4;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddRoles<IdentityRole>()//di for roles
                .AddEntityFrameworkStores<PanierContext>();

            var redisSettings = new RedisCacheSettings();
            Configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisSettings);
            services.AddSingleton(redisSettings);
            services.AddSingleton<IRedisConnection, RedisConnection>(q => new RedisConnection(redisSettings.ConnectionString));

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBasketItemRepository, BasketItemRepository>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
            services.AddScoped<IBasketItemtService, BasketItemtService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();

            services.AddTransient<IRedisRepository, RedisRepository>();

       
            #region token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            //to reach this added as singleton service to service container
            services.AddSingleton(tokenValidationParameters);
            #endregion

            services.AddAuthentication(configureOptions: x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;

            });


            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo() { Title = "Panier", Version = "v1" });

                //s.ExampleFilters();//adds example filters

                s.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme
                {
                    Description = "Jwt Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }

                });

         

            });


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
