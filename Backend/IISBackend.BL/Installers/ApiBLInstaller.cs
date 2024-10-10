using IISBackend.BL.Facades.Interfaces;
using IISBackend.DAL.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;


namespace IISBackend.BL.Installers
{
    public class ApiBLInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            serviceCollection.Scan(selector => selector
                .FromAssemblyOf<ApiBLInstaller>()
                .AddClasses(filter => filter.AssignableTo(typeof(IFacade<,,,>)))
                .AsMatchingInterface()
                .WithScopedLifetime());
        }
    }
}