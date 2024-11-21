//using IISBackend.BL.Services.Interfaces;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Oci.Common.Auth;
//using Oci.ObjectstorageService;
//using Oci.ObjectstorageService.Requests;
//using Oci.ObjectstorageService.Responses;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

//namespace IISBackend.BL.Services;

//public class OracleObjectStorageService : IObjectStorageService
//{
//    private static ObjectStorageClient client;
//    public OracleObjectStorageService()
//    {
//        var authProvider = new InstancePrincipalsAuthenticationDetailsProvider();
//        client = new ObjectStorageClient(authProvider);
//    }

//    public Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName, TimeSpan expiration)
//    {
//    }

//    public async Task<bool> UploadObjectAsync(string bucketName, string objectName, Stream content)
//    {
//        var objectRequest = new PutObjectRequest
//        {
//            BucketName = bucketName,
//            ObjectName = objectName,
//            PutObjectBody = content,
//        };
//        var response = client.PutObject(objectRequest);
//        return (await response).httpResponseMessage.IsSuccessStatusCode;
//    }

//    public Task<bool> ObjectExistsAsync(string bucketName, string objectName)
//    {

//    }

//    public Task DeleteObjectAsync(string bucketName, string objectName)
//    {

//    }

//    public async Task<Stream> GetFileStream(string bucketName, string objectName)
//    {

//    }
//}
