using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using IISBackend.BL.Models.Schedules;
using IISBackend.BL.Models.Requests;
using System.Security.Claims;
using System.Data.Entity.Core.Metadata.Edm;
using Microsoft.EntityFrameworkCore;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Facades;

public class ReservationRequestFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper, IAuthorizationService authService) : FacadeCRUDBase<ReservationRequestEntity, ReservationRequestCreateModel, ReservationRequestListModel, ReservationRequestDetailModel>(unitOfWorkFactory, modelMapper), IReservationRequestFacade
{

    private readonly IAuthorizationService _authService = authService;
    protected override ICollection<string> IncludesNavigationPathDetail => new[] { $"{nameof(ReservationRequestEntity.Animal)}", $"{nameof(ReservationRequestEntity.TargetUser)}",$"{nameof(ReservationRequestEntity.Creator)}" };

    private ScheduleEntryEntity? GetWalkSchedule(ReservationRequestCreateModel model, IUnitOfWork uow)
    {
        return uow.GetRepository<ScheduleEntryEntity>().Get().FirstOrDefault(o => o.Type == Common.Enums.ScheduleType.availableForWalk && o.AnimalId == model.AnimalId && o.Time == model.Time);
    }

    private async Task ValidateTargetUser(ReservationRequestEntity reservation, IUnitOfWork uow,UserEntity targetUserEntity)
    {
        switch(reservation.Type)
        {
            case ScheduleType.walk:
                if(reservation.CreatorID != reservation.TargetUserId)
                    throw new ArgumentException("Cannot create walk request for different user");
                break;
            case ScheduleType.vetVisit:
                if (!await uow.GetUserManager().IsInRoleAsync(targetUserEntity, "Vet"))
                    throw new ArgumentException("Target user is not Vet");
                break;
        }
    }

    public async Task<ReservationRequestDetailModel> AuthorizedCreateRequest(ReservationRequestCreateModel model, ClaimsPrincipal user)
    {
        var entity = _modelMapper.Map<ReservationRequestEntity>(model);


        if (await _authService.AuthorizeAsync(user, entity, "UserIsAllowedToRequest") is not { Succeeded: true })
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }

        await using var uow = _UOWFactory.Create();
        var requestRepository =  uow.GetRepository<ReservationRequestEntity>();
        var requestor = await uow.GetUserManager().GetUserAsync(user) ?? throw new UnauthorizedAccessException("Requesting user not found");
        entity.CreatorID = requestor.Id;
        if (requestRepository.Get().FirstOrDefault(o=>o.Id == entity.Id) is not null)
        {
            throw new ArgumentException("Request already exists");
        }

        var scheduleRepository = uow.GetRepository<ScheduleEntryEntity>();
        var animalRepository = uow.GetRepository<AnimalEntity>();
        var volunteerRepository = uow.GetRepository<UserEntity>();

        var animal = await animalRepository.Get().Include(x => x.ScheduleEntries).FirstOrDefaultAsync(o => o.Id == entity.AnimalId);
        var userEntity = await volunteerRepository.Get().Include(x => x.ScheduleEntries).FirstOrDefaultAsync(o => o.Id == entity.TargetUserId);
        if (animal is null || userEntity is null)
        {
            var messages = new List<string>();
            if (animal is null)
            {
                messages.Add("Animal not found");
            }
            if (userEntity is null)
            {
                messages.Add("Target user not found");
            }
            throw new ArgumentException(string.Join($"{Environment.NewLine}", messages));
        }

        if (model.Type == ScheduleType.walk)
        {
            var walkSchedule = GetWalkSchedule(model, uow);
            if (walkSchedule is null)
            {
                throw new ArgumentException("Available walk schedule not found");
            }
            entity.TargetScheduleId = walkSchedule.Id;
        }
        else
        {
            if (animal.ScheduleEntries!.Any(o => o.Time == model.Time) || userEntity.ScheduleEntries!.Any(o => o.Time == model.Time))
            {
                throw new ArgumentException("User or Animal already has schedule");
            }
        }

        await ValidateTargetUser(entity, uow, userEntity);

        try
        {
            await requestRepository.InsertAsync(entity);
            await uow.CommitAsync();
            return _modelMapper.Map<ReservationRequestDetailModel>(entity);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to create the reservation request", e);
        }
    }

    public async Task AuthorizedCancelRequest(Guid id, ClaimsPrincipal user)
    {
        await using var uow = _UOWFactory.Create();
        var requestRepository = uow.GetRepository<ReservationRequestEntity>();
        var request = await requestRepository.Get().Include(x => x.Animal).Include(x => x.TargetUser).Include(x => x.TargetSchedule).FirstOrDefaultAsync(o => o.Id == id);
        ScheduleEntryEntity? resultingSchedule = null;
        if (request is null)
        {
            throw new InvalidDataException("Request not found");
        }

        if (await _authService.AuthorizeAsync(user, request!, "UserIsAllowedToApproveRequest") is not { Succeeded: true } &&
            await _authService.AuthorizeAsync(user,request,"UserIsOwnerRequirement") is not { Succeeded: true})
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }


        await requestRepository.DeleteAsync(id);
        try
        {
            await uow.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to resolve the reservation request", e);
        }
    }

    public async Task<ScheduleDetailModel?> AuthorizedResolveRequest(Guid id, bool approved, ClaimsPrincipal user)
    {
        await using var uow = _UOWFactory.Create();
        var requestRepository = uow.GetRepository<ReservationRequestEntity>();
        var request = await requestRepository.Get().Include(x => x.Animal).Include(x => x.TargetUser).Include(x => x.TargetSchedule).FirstOrDefaultAsync(o => o.Id == id);
        ScheduleEntryEntity? resultingSchedule = null;
        if (request is null)
        {
            throw new InvalidDataException("Request not found");
        }

        if (await _authService.AuthorizeAsync(user, request!, "UserIsAllowedToApproveRequest") is not { Succeeded: true })
        {
            throw new UnauthorizedAccessException("User is not authorized");
        }



        if (approved)
        {
            if (request.Type == ScheduleType.walk)
            {
                var scheduleRepository = uow.GetRepository<ScheduleEntryEntity>();
                if (request.TargetSchedule is null) throw new InvalidOperationException("Request has no target schedule");
                await scheduleRepository.DeleteAsync(request.TargetSchedule.Id);
                resultingSchedule = await scheduleRepository.InsertAsync(_modelMapper.Map<ScheduleEntryEntity>(request));
            }
            else
            {
                var req = _modelMapper.Map<ScheduleEntryEntity>(request);
                resultingSchedule = await uow.GetRepository<ScheduleEntryEntity>().InsertAsync(req);
                var conflictingRequests = await requestRepository.Get().Where(o => o.Time == request.Time && o.AnimalId == request.AnimalId).ToListAsync();
                foreach (var conflictingRequest in conflictingRequests)
                {
                    await requestRepository.DeleteAsync(conflictingRequest.Id);
                }
            }
        }
        else
        {
            await requestRepository.DeleteAsync(id);
        }
        try
        {
            await uow.CommitAsync();
            return _modelMapper.Map<ScheduleDetailModel>(resultingSchedule);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to resolve the reservation request", e);
        }
    }
}
