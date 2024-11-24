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
using System.Data;
using IISBackend.DAL.Entities.Interfaces;
using System.Runtime.CompilerServices;
using IISBackend.BL.Models;


namespace IISBackend.BL.Facades;

public class UserFacade(IUnitOfWorkFactory _unitOfWorkFactory, IAuthorizationService _authService, IMapper _modelMapper) : IUserFacade
{
    public async Task<SignInResult> Login(LoginModel login)
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        var signInManager = uow.GetSignInManager();
        var user = await uow.GetUserManager().FindByNameAsync(login.Name);
        if (user == null)
        {
            return SignInResult.Failed;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        if (!signInResult.Succeeded)
        {
            return signInResult;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
        };

        await signInManager.SignInWithClaimsAsync(user, false, claims);
        return signInResult;
    }

    public async Task Logout()
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        var signInManager = uow.GetSignInManager();
        await signInManager.SignOutAsync();
    }

    public Guid? GetCurrentUserGuid(ClaimsPrincipal user)
    {
        return user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => Guid.Parse(c.Value)).FirstOrDefault();
    }

    public async Task<UserDetailModel?> GetUserByIdAsync(Guid id)
    {
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserEntity? entity = await uow.GetUserManager().Users.Include(u=>u.Image).FirstOrDefaultAsync(u=>id == u.Id).ConfigureAwait(false);
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
            .GetUserManager().Users.Include(u => u.Image)
            .ToListAsync().ConfigureAwait(false);
        var mapped = _modelMapper.Map<List<UserListModel>>(entities);
        for (int i = 0; i < mapped.Count; i++)
        {
            UserListModel? user = mapped[i];
            user.Roles = await uow.GetUserManager().GetRolesAsync(entities[i]);
        }
        return mapped;
    }

    public async Task<UserDetailModel?> CreateAsync(UserCreateModel model, string? roleName = null)
    {
        UserEntity entity = _modelMapper.Map<UserEntity>(model);
        UserDetailModel? result = null;

        ITransactionalUnitOfWork uow = _unitOfWorkFactory.CreateTransactional();
        UserManager<UserEntity> userManager = uow.GetUserManager();
        if (roleName != null && !await uow.GetRoleManager().RoleExistsAsync(roleName))
        {
            throw new ArgumentException("Role does not exist");
        }

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (existingUser != null)
        {
            throw new ArgumentException("UserId is already taken");

        }
        else
        {
            var creationResult = await userManager.CreateAsync(entity, model.Password).ConfigureAwait(false);
            await uow.SaveChangesAsync();

            if (!creationResult.Succeeded)
            {
                await uow.RevertChangesAsync();
                throw new ArgumentException(string.Join(Environment.NewLine, creationResult.Errors.Select(o => o.Description)));
            }

            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            if (roleName != null)
            {
                var resultUser = await userManager.FindByIdAsync(entity.Id.ToString());
                await userManager.AddToRoleAsync(resultUser!, roleName);
                await uow.SaveChangesAsync();
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

    private async Task CheckIfFileOperationIsValid(FileEntity? fileEntity , ClaimsPrincipal userPrincipal, UserEntity existingUser, IAuthorizationService authorizationService)
    {

        if (fileEntity == null)
        {
            throw new ArgumentException("Image not found");
        }
        else if (userPrincipal == null || !(await authorizationService.AuthorizeAsync(userPrincipal, fileEntity, "UserIsOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized to assign this image");
        }
    }

    public async Task<UserDetailModel?> UpdateAsync(UserUpdateModel model, ClaimsPrincipal userPrincipal)
    {
        UserDetailModel? result = null;

        IUnitOfWork uow = _unitOfWorkFactory.Create();
        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.Include(e => e.Image).FirstOrDefaultAsync(e => e.Id == model.Id).ConfigureAwait(false);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        if (model.ImageId != null)
        {
            var requestedImage = await uow.GetRepository<FileEntity>().Get().Where(e => e.Id == model.ImageId).FirstOrDefaultAsync() ?? throw new ArgumentException("Image not found");
            await CheckIfFileOperationIsValid(requestedImage, userPrincipal, existingUser, _authService);
        }

        // Check if the current user is trying to update their own profile
        userPrincipal = userPrincipal ?? throw new UnauthorizedAccessException("User is not authorized");
        UserEntity? requestingUser = await userManager.GetUserAsync(userPrincipal) ?? throw new UnauthorizedAccessException("User is not authorized");


        if (userPrincipal == null || !(await _authService.AuthorizeAsync(userPrincipal, requestingUser, "UserIsAccountOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        if (model.Roles != null)
        {
            foreach (var role in model.Roles)
            {
                if (!await uow.GetRoleManager().RoleExistsAsync(role))
                {
                    throw new ArgumentException("Role does not exist");
                }
            }

            if ((await _authService.AuthorizeAsync(userPrincipal, model, "UserAllowedToGiveRolePolicy")).Succeeded)
            {
                var roles = await userManager.GetRolesAsync(existingUser);
                await userManager.RemoveFromRolesAsync(existingUser, roles.Except(model.Roles));
                await userManager.AddToRolesAsync(existingUser, model.Roles.Except(roles));
            }
            else
            {
                throw new UnauthorizedAccessException("User is not authorized to change roles");

            }
        }


        if ((await userManager.UpdateAsync(_modelMapper.Map(model, existingUser)).ConfigureAwait(false)).Succeeded)
        {
            UserEntity? updatedEntity = await userManager.Users.FirstOrDefaultAsync(e => e.Id == model.Id);
            result = _modelMapper.Map<UserDetailModel>(updatedEntity);
        }

        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
            if (result != null)
                result.Roles = await userManager.GetRolesAsync(existingUser);
        }
        catch
        {
            throw new InvalidOperationException("An error occurred while committing the transaction.");
        }
        return result;
    }


    public async Task<UserDetailModel?> DeleteAsync(Guid id, ClaimsPrincipal userPrincipal)
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
        if (userPrincipal == null || !(await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsAccountOwnerPolicy")).Succeeded)
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
