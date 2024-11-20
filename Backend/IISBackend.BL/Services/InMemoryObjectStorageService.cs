using IISBackend.BL.Services.Facades;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IISBackend.BL.Services;

public class InMemoryObjectStorageService : IObjectStorageService
{
    private static Task? _server = null;
    private static string? _serverPath;
    public InMemoryObjectStorageService()
    {
        if (_server == null)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"appPath is {path}");
            if (path == "") path = "/app";
            string storagePath = Path.Combine(path, "MockBucket");
            if (Directory.Exists(storagePath) == false)
                Directory.CreateDirectory(storagePath);
            _serverPath = storagePath;
            _server = Task.Run(StartServer);
        }
    }

    public void StartServer()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();



        app.MapPut("/{bucketName}/{objectName}", async (string bucketName, string objectName, HttpRequest request) =>
        {
            string bucketPath = Path.Combine(_serverPath!, bucketName);
            if (Directory.Exists(bucketPath) == false)
                Directory.CreateDirectory(bucketPath);

            var filePath = Path.Combine(bucketPath, objectName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.Body.CopyToAsync(fileStream);
            }

            return Results.Ok($"File stored at {filePath}");
        });

        app.MapGet("/{bucketName}/{objectName}", async (string bucketName, string objectName) =>
        {
            string bucketPath = Path.Combine(_serverPath!, bucketName);
            var filePath = Path.Combine(bucketPath, objectName);

            if (File.Exists(filePath))
            {
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(objectName, out var contentType))
                {
                    contentType = "application/octet-stream"; // Fallback MIME type
                }
                return Results.File(filePath, contentType: contentType);
            }

            return Results.NotFound();
        });

        app.Run("http://0.0.0.0:5000");
    }

    public Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName, TimeSpan expiration)
    {
        // Simulate a presigned URL (in real life, this would be a signed URL to the bucket)
        var fakeUrl = $"http://localhost:5000/{bucketName}/{objectName}?expires={DateTime.UtcNow.Add(expiration):o}";
        return Task.FromResult(fakeUrl);
    }

    public Task UploadObjectAsync(string bucketName, string objectName, Stream content)
    {
        if(Directory.Exists(bucketName) == false)
        {
            Directory.CreateDirectory(bucketName);
        }

        using (var fileStream = File.Create(Path.Combine(_serverPath!,bucketName, objectName)))
        {
            content.CopyTo(fileStream);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ObjectExistsAsync(string bucketName, string objectName)
    {
        var result = File.Exists(Path.Combine(_serverPath!,bucketName, objectName));
        return Task.FromResult(result);
    }

    public Task DeleteObjectAsync(string bucketName, string objectName)
    {
        File.Delete(Path.Combine(_serverPath!, bucketName, objectName));
        return Task.CompletedTask;
    }
}
