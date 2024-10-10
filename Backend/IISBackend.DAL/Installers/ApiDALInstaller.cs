using AutoMapper;
using IISBackend.DAL.Migrators;
using IISBackend.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.DAL.Installers;

public class ApiDALInstaller
{
    public void Install(IServiceCollection serviceCollection, DALOptions dalOptions)
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
            serviceCollection.AddDbContext<ProjectDbContext>(x => x.UseInMemoryDatabase("testdb")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }
        serviceCollection.AddSingleton<DALOptions>();
        serviceCollection.AddScoped<IDbMigrator, DbMigrator>();

    }
}
