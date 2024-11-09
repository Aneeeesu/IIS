using IISBackend.DAL.Entities;

namespace IISBackend.BL.Models.Schedules;

public record ScheduleDetailModel : ScheduleBaseModel
{
    public required UserEntity Volunteer { get; set; }
    public required AnimalEntity Animal { get; set; }
}