using IISBackend.API.Authorization;
using IISBackend.BL.Authorization;
using IISBackend.BL.Facades;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Options;
using IISBackend.BL.Services;
using IISBackend.BL.Services.Interfaces;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Options;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace IISBackend.BL.Installers
{
    public class ApiBLInstaller
    {
        public void Install(IServiceCollection serviceCollection, bool development)
        {
            serviceCollection.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            serviceCollection.Scan(selector => selector
                .FromAssemblyOf<ApiBLInstaller>()
                .AddClasses(filter => filter.AssignableToAny(typeof(IFacade)))
                .AsMatchingInterface()
                .WithScopedLifetime());
            serviceCollection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("UserIsAccountOwnerPolicy", policy =>
                    policy.Requirements.Add(new UserIsAccountOwnerRequirement()));
                options.AddPolicy("UserIsOwnerPolicy", policy =>
                    policy.Requirements.Add(new UserIsOwnerRequirement()));
                options.AddPolicy("UserAllowedToGiveRolePolicy", policy =>
                    policy.Requirements.Add(new UserAllowedToGiveRoleRequirement()));
                options.AddPolicy("UserIsAllowedToRequest", policy =>
                    policy.Requirements.Add(new UserIsAllowedToRequestRequirement()));
                options.AddPolicy("UserIsAllowedToApproveRequest", policy =>
                    policy.Requirements.Add(new UserIsAllowedToApproveRequestRequirement()));
                options.AddPolicy("UserAllowedToManageSchedulePolicy", policy =>
                    policy.Requirements.Add(new UserAllowedToManageScheduleRequirement()));
            });

            if (development)
            {
                serviceCollection.AddHostedService<MockObjectStorageHttpServerService>();
                serviceCollection.AddSingleton<IObjectStorageService, InMemoryObjectStorageService>();
            }
            else
                serviceCollection.AddSingleton<IObjectStorageService, OracleObjectStorageService>();

            serviceCollection.AddSingleton<IAuthorizationHandler, UserIsAccountOwnerAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, UserIsOwnerAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, UserAllowedToGiveRoleAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, UserIsAllowedToRequestAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, UserIsAllowedToApproveRequestAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, UserAllowedToManageScheduleAuthorizationHandler>();
            serviceCollection.AddHostedService<UnusedFilesCleanupService>();
            serviceCollection.AddSingleton<FileStorageOptions>(new FileStorageOptions { BucketName = "IIS-Bucket", StorageNamespace = "frcuhx7ewbeu" });
        }
    }
}