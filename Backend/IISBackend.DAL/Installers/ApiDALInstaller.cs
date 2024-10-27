using AutoMapper;
using IISBackend.DAL.Authorization;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Migrators;
using IISBackend.DAL.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.DAL.Installers;

public class ApiDALInstaller
{
    public void Install(IServiceCollection serviceCollection, DALOptions dalOptions, Action<IdentityBuilder>? identityBuilder = null)
    {
        if (dalOptions is null)
        {
            throw new InvalidOperationException("No persistence provider configured");
        }

        if (string.IsNullOrEmpty(dalOptions.ConnectionString))
        {
            throw new InvalidOperationException($"{nameof(dalOptions.ConnectionString)} is not set");
        }

        if (!dalOptions.TestEnvironment)
        {
            serviceCollection.AddDbContext<ProjectDbContext>(options => options.UseSqlServer(dalOptions.ConnectionString));
        }
        else
        {
            serviceCollection.AddDbContext<ProjectDbContext>(x => x.UseInMemoryDatabase("testdb"));
        }
        serviceCollection.AddSingleton<DALOptions>(dalOptions);
        serviceCollection.AddScoped<IDbMigrator, DbMigrator>();

        var builder = serviceCollection.AddIdentityCore<UserEntity>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
                o.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ProjectDbContext>()
            .AddUserStore<ProjectUserStore>()
            .AddRoles<RoleEntity>()
            .AddRoleStore<ProjectRoleStore>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<UserEntity>>();

        if (identityBuilder != null)
        {
            identityBuilder(builder);
        }
    }
}
