using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Models.Schedules;
using IISBackend.DAL.Entities;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IScheduleFacade : IFacadeCRUD<ScheduleEntryEntity, ScheduleCreateModel, ScheduleListModel, ScheduleDetailModel>
{
    Task AuthorizedDeleteAsync(Guid id, ClaimsPrincipal userPrincipal);

    Task<List<ScheduleListModel>> GetAnimalSchedulesAsync(Guid animalId);
}