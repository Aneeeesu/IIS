using AutoMapper;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.File;
using IISBackend.BL.Services.Interfaces;
using IISBackend.DAL.Entities;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;
using System.Security.Claims;

namespace IISBackend.BL.Facades;

public class FileFacade(IUnitOfWorkFactory uowFactory,IObjectStorageService objectStorage,IMapper mapper,IAuthorizationService authService) : IFileFacade
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

    public async Task DeleteUnusedFiles(TimeSpan timeSpan)
    {
        try
        {
            await using IUnitOfWork uow = _uowFactory.Create();
            var fileRepository = uow.GetRepository<FileEntity>();
            var files = await fileRepository.Get().Include(x=>x.UserImages).Include(x=>x.AnimalImages).Where(f => f.AnimalImages!.Count == 0 && f.UserImages!.Count == 0 && f.UploadDate < DateTime.UtcNow.Add(timeSpan)).ToListAsync();
            foreach (var file in files)
            {
                await _objectStorage.DeleteObjectAsync("mockBucket", file.Url);
                await fileRepository.DeleteAsync(file.Id);
            }
            await uow.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to delete unused files", e);
        }
    }

    public async Task<ICollection<FileDetailModel>> GetFilesAsync()
    {
        await using IUnitOfWork uow = _uowFactory.Create();
        var fileRepository = uow.GetRepository<FileEntity>();
        var files = await fileRepository.Get().Include(x=>x.AnimalImages).Include(x=>x.UserImages).ToListAsync();
        return _mapper.Map<ICollection<FileDetailModel>>(files);
    }
}
