using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Extensions;
using IISBackend.DAL.Installers;
using AutoMapper;
using IISBackend.BL.Installers;
using IISBackend.BL.Facades.Extensions;
using IISBackend.DAL.Options;
using IISBackend.DAL.Migrators;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
ConfigureControllers(builder.Services);
ConfigureDependencies(builder.Services, builder.Configuration, builder.Environment.IsEnvironment("Test"));
ConfigureAutoMapper(builder.Services);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true/*app.Environment.IsDevelopment()*/)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

UseSecurityFeatures(app);


app.MapControllers();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IDbMigrator>().Migrate();
app.Run();

void ConfigureAutoMapper(IServiceCollection serviceCollection)
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(ApiDALInstaller).Assembly);
        cfg.AddMaps(typeof(ApiBLInstaller).Assembly);
    });
    serviceCollection.AddSingleton<IMapper>(config.CreateMapper());
}

void ConfigureDependencies(IServiceCollection serviceCollection, IConfiguration configuration, bool testEnvironment)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (connectionString is null && !testEnvironment)
    {
        throw new ArgumentException("The connection string is missing");
    }

    //new DALOptions

    serviceCollection.AddInstaller<ApiDALInstaller>(new DALOptions
    {
        ConnectionString = connectionString ?? String.Empty,
        TestEnvironment = testEnvironment
    });
    //serviceCollection.AddInstaller<ApiDALEFInstaller>(connectionString ?? String.Empty, testEnvironment);

    serviceCollection.AddInstaller<ApiBLInstaller>();
}

void UseSecurityFeatures(IApplicationBuilder application)
{
    application.UseCors();
}



void ConfigureControllers(IServiceCollection serviceCollection)
{
    serviceCollection.AddControllers()
        .AddDataAnnotationsLocalization()
        .AddJsonOptions(configure =>
            configure.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter())
            );

    serviceCollection.AddCors(options =>
    {
        options.AddDefaultPolicy(options =>
            options.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

}

public partial class Program
{

}