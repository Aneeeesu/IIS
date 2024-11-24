using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Models.Schedules;
using IISBackend.DAL.Entities;
using System.Collections;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IScheduleFacade : IFacadeCRUD<ScheduleEntryEntity, ScheduleCreateModel, ScheduleListModel, ScheduleDetailModel>
{
    Task<ScheduleDetailModel> AuthorizedCreateAsync(ScheduleCreateModel model, ClaimsPrincipal userPrincipal);
    Task AuthorizedCancelAsync(Guid id, ClaimsPrincipal userPrincipal);

    Task<ICollection<ScheduleListModel>> GetAnimalSchedulesAsync(Guid animalId);

    Task<ICollection<ScheduleListModel>> GetUserSchedulesAsync(Guid volunteerId);
}