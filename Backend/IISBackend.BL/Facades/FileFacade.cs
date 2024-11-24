using AutoMapper;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.File;
using IISBackend.BL.Options;
using IISBackend.BL.Services.Interfaces;
using IISBackend.DAL.Entities;
using IISBackend.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Security.Claims;

namespace IISBackend.BL.Facades;

public class FileFacade(IUnitOfWorkFactory uowFactory,IObjectStorageService objectStorage,IMapper mapper,IAuthorizationService authService,FileStorageOptions options) : IFileFacade
{
    private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
    private readonly IObjectStorageService _objectStorage = objectStorage;
    private readonly IMapper _mapper = mapper;

    private static readonly string[] AllowedFileTypes = { ".png", ".jpg", ".jpeg", ".gif" };

    public async Task<PendingFileUploadModel> GeneratePresignedUrlAsync(string fileType, TimeSpan expiration, ClaimsPrincipal User)
    {
        var guid = Guid.NewGuid();
        if (string.IsNullOrEmpty(fileType))
            throw new ArgumentException("File name cannot be empty", nameof(fileType));
        if(!AllowedFileTypes.Contains(fileType))
            throw new ArgumentException("Invalid file type", nameof(fileType));
        var url = await _objectStorage.GeneratePresignedUrlAsync(options.BucketName, guid.ToString() + fileType, expiration,true);

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

    public async Task<FileBaseModel> ValiadateFileUpload(Guid pendingRequestGuid,ClaimsPrincipal User)
    {

        await using IUnitOfWork uow = _uowFactory.Create();
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


        if (file.ExpirationDate < DateTime.UtcNow || !await _objectStorage.ObjectExistsAsync(options.BucketName, file.Key))
        {
            await pendingfileRepository.DeleteAsync(file.Id);
            await uow.CommitAsync();
            throw new ArgumentException("FileUploadExpired");
        }

        var readUrl = await _objectStorage.GeneratePresignedUrlAsync(options.BucketName, file.Key, new DateTime(2100,1,1) - DateTime.Now);

        var fileRepository = uow.GetRepository<FileEntity>();
        var result = await fileRepository.InsertAsync(new FileEntity
        {
            Id = pendingRequestGuid,
            OwnerId = file.UploaderId,
            UploadDate = DateTime.UtcNow,
            FileType = "image/png",
            Url = readUrl
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

        return _mapper.Map<FileBaseModel>(result);
    }

    public async Task DeleteUnusedFiles(TimeSpan timeSpan)
    {
        try
        {
            await using IUnitOfWork uow = _uowFactory.Create();
            var fileRepository = uow.GetRepository<FileEntity>();
            var expirationDate = DateTime.UtcNow.Add(timeSpan);
            var files = await fileRepository.Get().Include(x=>x.UserImages).Include(x=>x.AnimalImages).Where(f => f.AnimalImages!.Count == 0 && f.UserImages!.Count == 0 && f.UploadDate < expirationDate).ToListAsync();
            foreach (var file in files)
            {
                await _objectStorage.DeleteObjectAsync(options.BucketName, file.Url);
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
