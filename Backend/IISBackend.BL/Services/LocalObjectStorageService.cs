using IISBackend.BL.Options;
using IISBackend.BL.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Oci.ObjectstorageService.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IISBackend.BL.Services;

public class LocalObjectStorageService : IObjectStorageService
{
    private string _filePath;
    public LocalObjectStorageService(FileStorageOptions options)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IIS-Backend");
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        Console.WriteLine($"appPath is {path}");
        if (path == "") path = "/app";
        _filePath = path;
    }

    public Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName, TimeSpan expiration, bool write = false)
    {
        // Simulate a presigned URL (in real life, this would be a signed URL to the bucket)
        var fakeUrl = $"http://localhost:5000/{bucketName}/{objectName}";
        return Task.FromResult(fakeUrl);
    }

    public Task<bool> UploadObjectAsync(string bucketName, string objectName, Stream content)
    {
        if(!Directory.Exists(Path.Combine(_filePath,bucketName)))
        {
            Directory.CreateDirectory(Path.Combine(_filePath, bucketName));
        }

        using (var fileStream = File.Create(Path.Combine(_filePath!,bucketName, objectName)))
        {
            content.CopyTo(fileStream);
        }

        return Task.FromResult(true);
    }

    public Task<bool> ObjectExistsAsync(string bucketName, string objectName)
    {
        var result = File.Exists(Path.Combine(_filePath!,bucketName, objectName));
        return Task.FromResult(result);
    }

    public Task DeleteObjectAsync(string bucketName, string objectName)
    {
        File.Delete(Path.Combine(_filePath!, bucketName, objectName));
        return Task.CompletedTask;
    }

    public async Task<Stream> GetFileStream(string bucketName, string objectName)
    {
        var filePath = Path.Combine(_filePath!, bucketName, objectName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The requested file does not exist.", objectName);
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = File.OpenRead(filePath))
        {
            await fileStream.CopyToAsync(memoryStream);
        }

        memoryStream.Position = 0; // Reset the position for reading
        return memoryStream;
    }
}
