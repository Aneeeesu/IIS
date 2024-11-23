using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISBackend.BL.Services.Interfaces;

public interface IObjectStorageService
{
    Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName, TimeSpan expiration, bool write = false);
    Task<bool> UploadObjectAsync(string bucketName, string objectName, Stream content);
    Task<bool> ObjectExistsAsync(string bucketName, string objectName);
    Task DeleteObjectAsync(string bucketName, string objectName);
    Task<Stream> GetFileStream(string bucketName, string objectName);
}
