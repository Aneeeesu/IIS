using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Extensions;
using IISBackend.DAL.Installers;
using AutoMapper;
using IISBackend.BL.Installers;
using IISBackend.BL.Facades.Extensions;
using IISBackend.DAL.Options;
using IISBackend.DAL.Migrators;
using Microsoft.AspNetCore.Identity;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Seeds;
using IISBackend.BL.Services.Interfaces;
using IISBackend.BL.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var useMockBuket = bool.TryParse(builder.Configuration["DevelopmentBucket:Enabled"],out bool result) ? result : false;
// Add services to the container.

ConfigureControllers(builder.Services, builder.Configuration["FrontendAddress"] ?? "http://localhost:3000");
ConfigureDependencies(
    builder.Services,
    builder.Configuration,
    builder.Environment.IsEnvironment("Development"),
    useMockBuket,
    new FileStorageOptions
    {
        BucketName = builder.Configuration["BucketName"] ?? "IIS-Bucket",
        StorageNamespace = builder.Configuration["BucketNamespace"] ?? ""
    }
);

ConfigureAutoMapper(builder.Services);

builder.Services.AddControllers();
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = IdentityConstants.ApplicationScheme;
    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies(o => {
    o.ApplicationCookie?.Configure(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                // Instead of redirecting to login, return 403
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                // Instead of redirecting to access denied page, return 403
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger().RequireAuthorization();
}

UseSecurityFeatures(app);

app.MapControllers();

using var scope = app.Services.CreateScope();
var store = scope.ServiceProvider.GetRequiredService<IUserStore<UserEntity>>();
scope.ServiceProvider.GetRequiredService<IDbMigrator>().Migrate();
scope.ServiceProvider.GetRequiredService<DBSeeder>().Seed(app.Configuration.GetValue<string>("DefaultAdminPassword")??throw new NullReferenceException("Missing admin password in configuration"));
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

void ConfigureDependencies(IServiceCollection serviceCollection, IConfiguration configuration, bool testEnvironment,bool useMockBucket,FileStorageOptions bucketNameSpace)
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
        TestEnvironment = testEnvironment,
        SeedData = testEnvironment
    });
    serviceCollection.AddScoped<DBSeeder>();

    serviceCollection.AddInstaller<ApiBLInstaller>(useMockBucket, bucketNameSpace);
}

void UseSecurityFeatures(IApplicationBuilder application)
{
    application.UseCors();
    application.UseRouting();
    application.UseAuthentication();
    application.UseAuthorization();
}



void ConfigureControllers(IServiceCollection serviceCollection,string frontendAddress)
{
    serviceCollection.AddControllers()
        .AddDataAnnotationsLocalization()
        .AddJsonOptions(configure =>
            configure.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter())
            );

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(
                        frontendAddress)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition")
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
                    .AllowCredentials();


            }
        );
    });

}

public partial class Program
{

}