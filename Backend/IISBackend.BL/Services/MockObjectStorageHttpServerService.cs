using IISBackend.BL.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

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
        return app.RunAsync("http://0.0.0.0:5000");
    }
}
