using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Panier.Business.Options;
using Panier.Business.Services.Abstract;
using Panier.Business.Services.Concrete;
using Panier.Filters;
using System;
using System.Text;


namespace Panier.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            #region jwt settings
            var jwtSettings = new JwtSettings();
            configuration.Bind(key: nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("jwtsettings"));
            #endregion

            services.AddScoped<IIdentityService, IdentityService>();

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


            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();

            }).AddFluentValidation(mvcConfiguration => mvcConfiguration
                .RegisterValidatorsFromAssemblyContaining<Startup>())
             .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


        }
    }
}
