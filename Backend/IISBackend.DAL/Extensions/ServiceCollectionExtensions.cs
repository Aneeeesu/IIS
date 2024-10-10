using IISBackend.DAL.Installers;
using IISBackend.DAL.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.DAL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(this IServiceCollection serviceCollection, DALOptions dalOptions)
            where TInstaller : ApiDALInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection, dalOptions);
        }
    }
}
