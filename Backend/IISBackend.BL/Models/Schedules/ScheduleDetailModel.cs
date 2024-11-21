using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Models.Schedules;

public record ScheduleDetailModel : ScheduleBaseModel
{
    public required UserListModel User { get; set; }
    public required AnimalListModel Animal { get; set; }
}