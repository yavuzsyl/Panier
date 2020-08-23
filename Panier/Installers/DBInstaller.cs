using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Panier.DataAccess.Data;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Panier.Installers
{
    public class DBInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContextPool<PanierContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PanierConnection")));
            services.AddDefaultIdentity<AppUser>(opts => {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 4;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PanierContext>();

        }
    }
}
