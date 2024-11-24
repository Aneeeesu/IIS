using IISBackend.BL.Installers;
using IISBackend.BL.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.BL.Facades.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(this IServiceCollection serviceCollection,bool isDevelopment,FileStorageOptions storageOptions)
            where TInstaller : ApiBLInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection,isDevelopment,storageOptions);
        }
    }
}
