using IISBackend.BL.Models.File;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IFileFacade : IFacade
{
    Task<Guid?> ValiadateFileUpload(Guid pendingRequestGuid, ClaimsPrincipal User);
    Task<PendingFileUploadModel> GeneratePresignedUrlAsync(string bucketName, string fileName, TimeSpan expiration, ClaimsPrincipal User);
    Task DeleteUnusedFiles(TimeSpan expiration);
    Task<ICollection<FileDetailModel>> GetFilesAsync();
}