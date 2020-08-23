using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            //get all classes that implements IInstaller and create instances of them then cast hem to IInstaller - IInstaller interfaceini implemente eden sınıfları çek ve instancelarını oluştur
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
                  typeof(IInstaller).IsAssignableFrom(x) && !x.IsAbstract && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            //service containerı içine gerekeli serviceler eklenir
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
