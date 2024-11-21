namespace IISBackend.BL.Models.Schedules;

public record ScheduleListModel : ScheduleBaseModel
{
    public required Guid UserId { get; set; }
    public required Guid AnimalId { get; set; }
}
