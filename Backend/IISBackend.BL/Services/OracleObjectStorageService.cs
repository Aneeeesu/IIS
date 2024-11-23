using IISBackend.BL.Options;
using IISBackend.BL.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Models;
using Oci.ObjectstorageService.Requests;
using Oci.ObjectstorageService.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IISBackend.BL.Services;

public class OracleObjectStorageService : IObjectStorageService
{
    private static ObjectStorageClient? client;
    private readonly FileStorageOptions _options;
    public OracleObjectStorageService(FileStorageOptions options)
    {
        var authProvider = new InstancePrincipalsAuthenticationDetailsProvider();
        client = new ObjectStorageClient(authProvider);
        _options = options;
    }

    public async Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName, TimeSpan expiration,bool write = false)
    {
        var request = new CreatePreauthenticatedRequestRequest
        {
            BucketName = bucketName,
            NamespaceName = _options.StorageNamespace,
            CreatePreauthenticatedRequestDetails = new CreatePreauthenticatedRequestDetails
            {
                Name = bucketName + "_ISS-API",
                AccessType = write ? CreatePreauthenticatedRequestDetails.AccessTypeEnum.ObjectWrite : CreatePreauthenticatedRequestDetails.AccessTypeEnum.ObjectRead,
                ObjectName = objectName, // Ensure this is set to the object's key if needed
                TimeExpires = DateTime.UtcNow.Add(expiration),
            },
        };
        var response = client!.CreatePreauthenticatedRequest(request);
        return (await response).PreauthenticatedRequest.FullPath;
    }

    public async Task<bool> UploadObjectAsync(string bucketName, string objectName, Stream content)
    {
        var objectRequest = new PutObjectRequest
        {
            NamespaceName = _options.StorageNamespace,
            BucketName = bucketName,
            ObjectName = objectName,
            PutObjectBody = content,
        };
        var response = client!.PutObject(objectRequest);
        return (await response).httpResponseMessage.IsSuccessStatusCode;
    }

    public async Task<bool> ObjectExistsAsync(string bucketName, string objectName)
    {
        var objectRequest = new HeadObjectRequest
        {
            NamespaceName = _options.StorageNamespace,
            BucketName = bucketName,
            ObjectName = objectName,
        };
        var response = client!.HeadObject(objectRequest);
        return (await response).httpResponseMessage.IsSuccessStatusCode;
    }

    public async Task DeleteObjectAsync(string bucketName, string objectName)
    {
        var objectRequest = new DeleteObjectRequest
        {
            NamespaceName = _options.StorageNamespace,
            BucketName = bucketName,
            ObjectName = objectName,
        };
        var response = client!.DeleteObject(objectRequest);
        await response;
    }

    public async Task<Stream> GetFileStream(string bucketName, string objectName)
    {
        var objectRequest = new GetObjectRequest
        {
            NamespaceName = _options.StorageNamespace,
            BucketName = bucketName,
            ObjectName = objectName,
        };
        var response = client!.GetObject(objectRequest);
        return (await response).InputStream;
    }
}
