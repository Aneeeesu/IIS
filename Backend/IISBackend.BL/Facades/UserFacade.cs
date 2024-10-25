using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using Microsoft.AspNetCore.Identity;
using MySqlX.XDevAPI.Common;
using System.Security.Claims;
using IISBackend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using IISBackend.BL.Models.User;
using Microsoft.AspNetCore.Authorization;

namespace IISBackend.BL.Facades;

public class UserFacade(IUnitOfWorkFactory _unitOfWorkFactory, IAuthorizationService _authService, IMapper _modelMapper) : IUserFacade
{
    //protected override ICollection<string> IncludesNavigationPathDetail =>
    //    new[] { $"{nameof(ActivityEntity.Subject)}", $"{nameof(ActivityEntity.Scores)}" };

    public async Task<List<UserDetailModel>> GetAsync()
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        List<UserEntity> entities = await uow
            .GetUserManager().Users
            .ToListAsync().ConfigureAwait(false);

        return _modelMapper.Map<List<UserDetailModel>>(entities);
    }

    public async Task<UserDetailModel?> CreateAsync(UserCreateModel model)
    {
        UserEntity entity = _modelMapper.Map<UserEntity>(model);
        UserDetailModel? result = null;

        IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (existingUser != null)
        {
            throw new ArgumentException("UserId is already taken");

        }
        else
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            var creationResult = await userManager.CreateAsync(entity, model.Password).ConfigureAwait(false);
            if (!creationResult.Succeeded)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, creationResult.Errors.Select(o => o.Description)));
            }
            result = _modelMapper.Map<UserDetailModel>(entity);
        }
        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while committing the transaction.", ex);
        }
        return result;
    }



    public async Task<UserDetailModel?> UpdateAsync(UserUpdateModel model, ClaimsPrincipal? userPrincipal = null)
    {
        UserEntity entity = _modelMapper.Map<UserEntity>(model);
        UserDetailModel? result = null;

        IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id).ConfigureAwait(false);
        // Check if the current user is trying to update their own profile
        if (userPrincipal == null || !(await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        if (existingUser != null)
        {
            _modelMapper.Map(entity, existingUser);
            if ((await userManager.UpdateAsync(existingUser).ConfigureAwait(false)).Succeeded)
            {
                UserEntity? updatedEntity = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);
                result = _modelMapper.Map<UserDetailModel>(updatedEntity);
            }
        }
        else
        {
            throw new ArgumentException("User not found");
        }
        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
        return result;
    }
}
