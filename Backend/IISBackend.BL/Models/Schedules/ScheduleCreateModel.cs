namespace IISBackend.BL.Models.Schedules;

public record ScheduleCreateModel : ScheduleBaseModel
{
    public Guid? UserId { get; set; }
    public required Guid AnimalId { get; set; }
}
