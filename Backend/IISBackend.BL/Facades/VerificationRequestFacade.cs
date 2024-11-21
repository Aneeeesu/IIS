using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using System.Security.Claims;
using IISBackend.BL.Models.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.BL.Facades;

public class VerificationRequestFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper, IAuthorizationService authService) : FacadeCRUDBase<VerificationRequestEntity, VerificationRequestCreateModel, VerificationRequestListModel, VerificationRequestDetailModel>(unitOfWorkFactory, modelMapper), IVerificationRequestFacade
{
    private readonly IAuthorizationService _authService = authService;

    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { $"{nameof(VerificationRequestEntity.Requestee)}", $"{nameof(VerificationRequestEntity.Requestee)}.{nameof(UserEntity.Image)}" };

    public override async Task<IEnumerable<VerificationRequestListModel>> GetAsync()
    {
        await using IUnitOfWork uow = _UOWFactory.Create();
        IQueryable<VerificationRequestEntity> query = uow.GetRepository<VerificationRequestEntity>().Get();
        query = query.Include(r=>r.Requestee).ThenInclude(u=>u!.Image);
        var entities = await query.ToListAsync().ConfigureAwait(false);
        var mappedEntities = _modelMapper.Map<List<VerificationRequestListModel>>(entities);
        return mappedEntities;
    }
    public override async Task<VerificationRequestDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();
        IQueryable<VerificationRequestEntity> query = uow.GetRepository<VerificationRequestEntity>().Get();
        query = query.Include(r => r.Requestee).ThenInclude(u => u!.Image);
        var entity = await query.FirstOrDefaultAsync(e=>e.Id == id).ConfigureAwait(false);
        var mappedEntity = _modelMapper.Map<VerificationRequestDetailModel>(entity);
        if (entity is null)
            return null;

        mappedEntity.Requestee.Roles = await uow.GetUserManager().GetRolesAsync(entity.Requestee!);
        return mappedEntity;
    }


    public async Task<VerificationRequestDetailModel?> AuthorizedCreateAsync(VerificationRequestCreateModel model, ClaimsPrincipal userPrincipal)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var user = await uow.GetUserManager().GetUserAsync(userPrincipal);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == model.RequesteeID).ConfigureAwait(false);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        var requestRepository = uow.GetRepository<VerificationRequestEntity>();
        var request = base._modelMapper.Map<VerificationRequestEntity>(model);
        request.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

        if (requestRepository.Get().FirstOrDefault(o => o.Id == request.Id) is not null)
        {
            throw new ArgumentException("Id is already taken");
        }


        // Check if the current user is trying to update their own profile
        if (userPrincipal == null || !(await _authService.AuthorizeAsync(userPrincipal, request, "UserIsOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        request = await requestRepository.InsertAsync(request).ConfigureAwait(false);

        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            throw new InvalidOperationException("Request not created");
        }

        return base._modelMapper.Map<VerificationRequestDetailModel>(request);
    }

    public async Task AuthorizedDeleteAsync(Guid id, ClaimsPrincipal userPrincipal)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var user = await uow.GetUserManager().GetUserAsync(userPrincipal);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        UserManager<UserEntity> userManager = uow.GetUserManager();

        var requestRepository = uow.GetRepository<VerificationRequestEntity>();
        var request = requestRepository.Get().FirstOrDefault(o => o.Id == id);
        if (request is null)
        {
            throw new ArgumentException("Request not found");
        }

        // Check if the current user is trying to update their own profile or is an admin
        if (userPrincipal == null || !(await _authService.AuthorizeAsync(userPrincipal, request, "UserIsOwnerPolicy")).Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        try
        {
            await requestRepository.DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            throw new InvalidOperationException("Request not deleted");
        }
    }

    public async Task ResolveRequestAsync(Guid id, bool approved)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var requestRepository = uow.GetRepository<VerificationRequestEntity>();
        var request = requestRepository.Get().FirstOrDefault(o => o.Id == id);
        if (request is null)
        {
            throw new ArgumentException("Request not found");
        }

        if (approved)
        {
            var userManager = uow.GetUserManager();
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == request.RequesteeID).ConfigureAwait(false);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (roles.Contains("Verified volunteer"))
            {
                throw new InvalidDataException("User is already verified");
            }
            await userManager.AddToRoleAsync(user, "Verified volunteer");
            await userManager.RemoveFromRoleAsync(user, "Volunteer");
        }

        try
        {
            await requestRepository.DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            throw new InvalidOperationException("Request not deleted");
        }
    }
}

