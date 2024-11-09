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
using System.Linq;


namespace IISBackend.BL.Facades;

public class UserFacade(IUnitOfWorkFactory _unitOfWorkFactory, IAuthorizationService _authService, IMapper _modelMapper) : IUserFacade
{

    public async Task<UserDetailModel?> GetUserByIdAsync(Guid id)
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserEntity? entity = await uow.GetUserManager().FindByIdAsync(id.ToString()).ConfigureAwait(false);
        if (entity == null)
        {
            return null;
        }
        UserDetailModel model = _modelMapper.Map<UserDetailModel>(entity);
        model.Roles = await uow.GetUserManager().GetRolesAsync(entity);
        return model;
    }

    public async Task<List<UserListModel>> GetAsync()
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        List<UserEntity> entities = await uow
            .GetUserManager().Users
            .ToListAsync().ConfigureAwait(false);

        return _modelMapper.Map<List<UserListModel>>(entities);
    }

    public async Task<UserDetailModel?> CreateAsync(UserCreateModel model, string? roleName = null)
    {
        UserEntity entity = _modelMapper.Map<UserEntity>(model);
        UserDetailModel? result = null;

        ITransactionalUnitOfWork uow = _unitOfWorkFactory.CreateTransactional();
        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (existingUser != null)
        {
            throw new ArgumentException("UserId is already taken");

        }
        else
        {
            var creationResult = await userManager.CreateAsync(entity, model.Password).ConfigureAwait(false);
            await uow.SaveChangesAsync();

            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            if (roleName != null)
            {
                await userManager.AddToRoleAsync(entity, roleName);
                await uow.SaveChangesAsync();
            }

            if (!creationResult.Succeeded)
            {
                await uow.RevertChangesAsync();
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
            await uow.RevertChangesAsync();
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
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        // Check if the current user is trying to update their own profile
        if (userPrincipal == null ||
            (!(await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsAccountOwnerPolicy")).Succeeded) && !await userManager.IsInRoleAsync(existingUser, "Admin"))
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }


        _modelMapper.Map(entity, existingUser);
        if ((await userManager.UpdateAsync(existingUser).ConfigureAwait(false)).Succeeded)
        {
            UserEntity? updatedEntity = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);
            result = _modelMapper.Map<UserDetailModel>(updatedEntity);
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


    public async Task<UserDetailModel?> DeleteAsync(Guid id, ClaimsPrincipal? userPrincipal = null)
    {
        UserDetailModel? result = null;

        IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        // Check if the current user is trying to update their own profile
        if (userPrincipal == null ||
            !(await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsAccountOwnerPolicy")).Succeeded || await userManager.IsInRoleAsync(existingUser,"Admin"))
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }


        if ((await userManager.DeleteAsync(existingUser).ConfigureAwait(false)).Succeeded)
        {
            result = _modelMapper.Map<UserDetailModel>(existingUser);
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
