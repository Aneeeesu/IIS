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

public class UserFacade(IUnitOfWorkFactory _unitOfWorkFactory, IAuthorizationService _authService, IMapper _modelMapper) : FacadeCRUDBase<UserEntity, UserCreateModel, UserListModel, UserDetailModel>(_unitOfWorkFactory, _modelMapper), IUserFacade
{
    //protected override ICollection<string> IncludesNavigationPathDetail =>
    //    new[] { $"{nameof(ActivityEntity.Subject)}", $"{nameof(ActivityEntity.Scores)}" };

    public override async Task<UserDetailModel?> SaveAsync(UserCreateModel model, ClaimsPrincipal? userPrincipal = null)
    {
        UserEntity entity = modelMapper.Map<UserEntity>(model);
        UserDetailModel result;

        IUnitOfWork uow = UOWFactory.Create();
        UserManager<UserEntity> UserManager = uow.GetUserManager<UserEntity>();

        var existingUser = await UserManager.Users.FirstOrDefaultAsync(e=>e.Id == entity.Id).ConfigureAwait(false);

        // Check if the current user is trying to update their own profile
        if (userPrincipal!=null && (await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsAuthorPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        if (existingUser != null)
        {
            if ((await UserManager.UpdateAsync(entity).ConfigureAwait(false)).Succeeded)
            {

                UserEntity? updatedEntity = await UserManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);
                result = modelMapper.Map<UserDetailModel>(updatedEntity);
            }
        }
        else
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            if ((await UserManager.CreateAsync(entity)).Succeeded)
            {
                UserEntity? insertedEntity = await UserManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);
                UserManager.AddPasswordAsync(insertedEntity, model.PasswordHash);
                result = modelMapper.Map<UserDetailModel>(insertedEntity);
            }
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
