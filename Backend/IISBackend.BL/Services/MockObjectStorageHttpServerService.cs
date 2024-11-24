using IISBackend.BL.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IISBackend.BL.Services;

public class MockObjectStorageHttpServerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    public MockObjectStorageHttpServerService(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _provider.CreateScope();
        var serverTask = StartServer(_provider.GetRequiredService<IObjectStorageService>(),stoppingToken);
        await serverTask;
    }
    public Task StartServer(IObjectStorageService storageService,CancellationToken stoppingToken)
    {
        var configuration = _provider.GetRequiredService<IConfiguration>();
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();

                }
            );
        });
        var app = builder.Build();
        app.UseCors();
        app.UseRouting();

        app.MapPut("/{bucketName}/{objectName}", async (string bucketName, string objectName, HttpRequest request) =>
        {
            using (var memoryStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset stream for reading
                await storageService.UploadObjectAsync(bucketName, objectName, memoryStream);
            }

            return Results.Ok($"File stored in bucket '{bucketName}' with name '{objectName}'");
        });
        app.MapMethods("/{bucketName}/{objectName}", new[] { "OPTIONS" }, (HttpResponse response) =>
        {
            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Headers["Access-Control-Allow-Methods"] = "PUT, OPTIONS";
            response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";

            return Results.Ok();
        });

        app.MapGet("/{bucketName}/{objectName}", async (string bucketName, string objectName) =>
        {
            if (await storageService.ObjectExistsAsync(bucketName, objectName))
            {
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(objectName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                var stream = await storageService.GetFileStream(bucketName, objectName);
                return Results.File(stream, contentType);
            }

            return Results.NotFound();
        });
        stoppingToken.Register(async () => await app.StopAsync());
        return app.RunAsync(configuration["DevelopmentBucket:Address"] ?? "http://0.0.0.0:5000");
    }
}
