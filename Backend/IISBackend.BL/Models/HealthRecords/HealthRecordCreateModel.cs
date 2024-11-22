namespace IISBackend.BL.Models.HealthRecords;

public class HealthRecordCreateModel : HealthRecordBaseModel
{
    public required Guid VetId { get; set; }
    public required Guid AnimalId { get; set; }
}
