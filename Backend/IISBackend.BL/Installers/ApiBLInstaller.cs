using IISBackend.API.Authorization;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
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
                .AddClasses(filter => filter.AssignableTo(typeof(IFacade)))
                .AddClasses(filter => filter.AssignableTo(typeof(IAuthorizationHandler)))
                .AsMatchingInterface()
                .WithScopedLifetime());
            serviceCollection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("EditPolicy", policy =>
                    policy.Requirements.Add(new UserIsOwnerRequirement()));
            });
        }
    }
}