using AutoMapper;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.File;
using IISBackend.BL.Services.Facades;
using IISBackend.DAL.Entities;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;
using System.Security.Claims;

namespace IISBackend.BL.Facades;

public class FileUploadFacade(IUnitOfWorkFactory uowFactory,IObjectStorageService objectStorage,IMapper mapper,IAuthorizationService authService) : IFileUploadFacade
{
    private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
    private readonly IObjectStorageService _objectStorage = objectStorage;
    private readonly IMapper _mapper = mapper;

    private static readonly string[] AllowedFileTypes = { ".png", ".jpg", ".jpeg", ".gif" };

    public async Task<PendingFileUploadModel> GeneratePresignedUrlAsync(string bucketName, string fileType, TimeSpan expiration, ClaimsPrincipal User)
    {
        var guid = Guid.NewGuid();
        if (string.IsNullOrEmpty(bucketName))
            throw new ArgumentException("Bucket name cannot be empty", nameof(bucketName));
        if (string.IsNullOrEmpty(fileType))
            throw new ArgumentException("File name cannot be empty", nameof(fileType));
        if(!AllowedFileTypes.Contains(fileType))
            throw new ArgumentException("Invalid file type", nameof(fileType));
        var url = await objectStorage.GeneratePresignedUrlAsync(bucketName, guid.ToString() + fileType, expiration);

        // Log the URL details
        var expiresAt = DateTime.UtcNow.Add(expiration);
        var uow = _uowFactory.Create();
        var user = uow.GetUserManager().GetUserAsync(User).Result ?? throw new UnauthorizedAccessException("User is not authorized");
        var fileRepository = uow.GetRepository<PendingFileUploadEntity>();
        var result = await fileRepository.InsertAsync(new PendingFileUploadEntity
        {
            UploaderId = user.Id,
            Id = guid,
            ExpirationDate = expiresAt,
            Key = guid.ToString() + fileType,
            Url = url
        });
        try { await uow.CommitAsync(); }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Failed to save the file upload", e);
        }
        catch (DbEntityValidationException e)
        {
            throw new InvalidOperationException("Failed to validate the file upload", e);
        }

        return _mapper.Map<PendingFileUploadModel>(result);
    }

    public async Task<Guid?> ValiadateFileUpload(Guid pendingRequestGuid,ClaimsPrincipal User)
    {

        await using IUnitOfWork uow = uowFactory.Create();
        var pendingfileRepository = uow.GetRepository<PendingFileUploadEntity>();
        var file = await pendingfileRepository.Get().FirstOrDefaultAsync(e => e.Id == pendingRequestGuid);

        if (file == null)
        {
            throw new ArgumentException("File not found");
        }

        if (!(await authService.AuthorizeAsync(User, file, "UserIsOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }


        if (file.ExpirationDate < DateTime.UtcNow || !await _objectStorage.ObjectExistsAsync("mockBucket", file.Key))
        {
            await pendingfileRepository.DeleteAsync(file.Id);
            await uow.CommitAsync();
            throw new ArgumentException("FileUploadExpired");
        }

        var fileRepository = uow.GetRepository<FileEntity>();
        var result = await fileRepository.InsertAsync(new FileEntity
        {
            Id = pendingRequestGuid,
            OwnerId = file.UploaderId,
            UploadDate = DateTime.UtcNow,
            FileType = "image/png",
            Url = file.Url,
            Used = false
        });
        try
        {
            await pendingfileRepository.DeleteAsync(file.Id);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Failed to save the file upload", e);
        }
        catch (DbEntityValidationException e)
        {
            throw new InvalidOperationException("Failed to validate the file upload", e);
        }

        return result.Id;
    }
}
