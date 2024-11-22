using IISBackend.BL.Models.HealthRecords;
using IISBackend.DAL.Entities;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IHealthRecordFacade : IFacadeCRUD<HealthRecordEntity, HealthRecordCreateModel, HealthRecordListModel, HealthRecordDetailModel>
{
    Task<HealthRecordDetailModel> AuthorizedCreateAsync(HealthRecordCreateModel record, ClaimsPrincipal user);
    Task<ICollection<HealthRecordListModel>> GetAnimalRecords(Guid animalID);
}