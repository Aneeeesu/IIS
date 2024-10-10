using IISBackend.BL.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.BL.Facades.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(this IServiceCollection serviceCollection)
            where TInstaller : ApiBLInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection);
        }
    }
}
