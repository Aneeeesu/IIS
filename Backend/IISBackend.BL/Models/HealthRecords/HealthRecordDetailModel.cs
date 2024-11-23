using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.HealthRecords;

public record HealthRecordDetailModel : HealthRecordBaseModel
{
    public required UserListModel Vet { get; set; }
    public required AnimalListModel Animal { get; set; }
}