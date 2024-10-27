using IISBackend.API.Authorization;
using IISBackend.BL.Facades;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.DAL.Entities;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                .AddClasses(filter => filter.AssignableToAny(typeof(IFacade)))
                .AsMatchingInterface()
                .WithScopedLifetime());
            serviceCollection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("UserIsOwnerPolicy", policy =>
                    policy.Requirements.Add(new UserIsOwnerRequirement()));
            });
            serviceCollection.AddSingleton<IAuthorizationHandler, UserIsOwnerAuthorizationHandler>();
        }
    }
}