using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Animal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IISBackend.BL.Models.Schedules;

namespace IISBackend.BL.Facades;

public class ScheduleFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper, IAuthorizationService authService) : FacadeCRUDBase<ScheduleEntryEntity, ScheduleCreateModel, ScheduleListModel, ScheduleDetailModel>(unitOfWorkFactory, modelMapper), IScheduleFacade
{
    private readonly IAuthorizationService _authService = authService;
    private readonly IMapper _modelMapper = modelMapper;


    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { $"{nameof(ScheduleEntryEntity.Animal)}", $"{nameof(ScheduleEntryEntity.User)}" };

    public async Task<ScheduleDetailModel> AuthorizedCreateAsync(ScheduleCreateModel model, ClaimsPrincipal userPrincipal)
    {
        var schedule = _modelMapper.Map<ScheduleEntryEntity>(model);
        if (await _authService.AuthorizeAsync(userPrincipal, schedule, "UserAllowedToManageSchedule") is not { Succeeded: true })
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        if (schedule.Type == Common.Enums.ScheduleType.availableForWalk && schedule.UserId != null)
        {
            throw new ArgumentException("Cannot create empty walk slot with assigned user");
        }

        await using IUnitOfWork uow = _UOWFactory.Create();
        var scheduleRepository = uow.GetRepository<ScheduleEntryEntity>();

        schedule.Id = schedule.Id == Guid.Empty ? Guid.NewGuid() : schedule.Id;

        var existingSchedule = scheduleRepository.Get().FirstOrDefault(o => o.Id == schedule.Id) ?? throw new ArgumentException("Schedule id is taken");
        existingSchedule = scheduleRepository.Get().FirstOrDefault(o => o.Time == schedule.Time && o.AnimalId == schedule.AnimalId) ?? throw new ArgumentException("Schedule already exists");

        ScheduleEntryEntity insertedSchedule = await uow.GetRepository<ScheduleEntryEntity>().InsertAsync(schedule);

        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch(Exception e)
        {
            throw new InvalidOperationException("Failed to save the schedule", e);
        }

        return _modelMapper.Map<ScheduleDetailModel>(insertedSchedule);
    }

    public async Task AuthorizedCancelAsync(Guid id, ClaimsPrincipal userPrincipal)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();
        var scheduleRepository = uow.GetRepository<ScheduleEntryEntity>();
        var schedule = scheduleRepository.Get().FirstOrDefault(o => o.Id == id);

        var user = await uow.GetUserManager().GetUserAsync(userPrincipal);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        if ((await _authService.AuthorizeAsync(userPrincipal, schedule, "UserIsOwnerPolicy")) is not { Succeeded: true } &&
            await _authService.AuthorizeAsync(userPrincipal, schedule, "UserAllowedToManageSchedule") is not { Succeeded: true })
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }


        UserManager<UserEntity> userManager = uow.GetUserManager();

        var existingUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        // Check if the current user is trying to update their own profile



        if (schedule is not null)
        {
            throw new ArgumentException("Schedule not found");
        }

        try
        {
            await scheduleRepository.DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Delete failed", e);
        }
    }

    public async Task<ICollection<ScheduleListModel>> GetAnimalSchedulesAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var user = await uow.GetUserManager().Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new ArgumentException("User not found");
        IQueryable<ScheduleEntryEntity> query = uow.GetRepository<ScheduleEntryEntity>().Get();
        query = query.Where(e => e.AnimalId == id);

        List<ScheduleEntryEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return base._modelMapper.Map<List<ScheduleListModel>>(entities);
    }

    public async Task<ICollection<ScheduleListModel>> GetVolunteerSchedulesAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        var user = await uow.GetUserManager().Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new ArgumentException("User not found");
        IQueryable<ScheduleEntryEntity> query = uow.GetRepository<ScheduleEntryEntity>().Get();
        query = query.Where(e => e.UserId == id);

        List<ScheduleEntryEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return base._modelMapper.Map<List<ScheduleListModel>>(entities);
    }
}
