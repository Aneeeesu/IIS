namespace IISBackend.BL.Models.Schedules;

public record ScheduleListModel : ScheduleBaseModel
{
    public required Guid VolunteerId { get; set; }
    public required Guid AnimalId { get; set; }
}
