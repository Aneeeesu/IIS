using IISBackend.BL.Models.Interfaces;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.HealthRecords;

public record HealthRecordBaseModel : IModel
{
    public Guid Id { get; init; }
    public required DateTime Time { get; set; }
    public required string Content { get; set; }
    public required HealthRecordType Type { get; set; }
}
