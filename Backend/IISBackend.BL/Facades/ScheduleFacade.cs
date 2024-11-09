using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Animal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IISBackend.DAL.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IISBackend.BL.Models.Schedules;

namespace IISBackend.BL.Facades;

public class ScheduleFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper, IAuthorizationService authService) : FacadeCRUDBase<ScheduleEntryEntity, ScheduleCreateModel, ScheduleListModel, ScheduleDetailModel>(unitOfWorkFactory, modelMapper), IScheduleFacade
{
    private readonly IAuthorizationService _authService = authService;
    private readonly IMapper _modelMapper = modelMapper;


    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { $"{nameof(ScheduleEntryEntity.Animal)}", $"{nameof(ScheduleEntryEntity.Volunteer)}" };



    public async Task AuthorizedDeleteAsync(Guid id, ClaimsPrincipal userPrincipal)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var user = await uow.GetUserManager().GetUserAsync(userPrincipal);


        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        // Check if the current user is trying to update their own profile
        if (userPrincipal == null ||
            (!(await _authService.AuthorizeAsync(userPrincipal, existingUser, "UserIsOwnerPolicy")).Succeeded && !(await userManager.GetRolesAsync(existingUser)).Any(s=>s=="Admin"|| s=="Vet")))
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        try
        {
            await uow.GetRepository<ScheduleEntryEntity>().DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Delete failed", e);
        }
    }

    public async Task<List<ScheduleListModel>> GetAnimalSchedulesAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        IQueryable<ScheduleEntryEntity> query = uow.GetRepository<ScheduleEntryEntity>().Get();

        query = query.Where(e => e.AnimalId == id);

        List<ScheduleEntryEntity> entities = await query.ToListAsync().ConfigureAwait(false);

        return modelMapper.Map<List<ScheduleListModel>>(entities);
    }
}