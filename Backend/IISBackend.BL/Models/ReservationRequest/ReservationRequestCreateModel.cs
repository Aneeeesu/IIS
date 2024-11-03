using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.ReservationRequest;

public record ReservationRequestCreateModel : ModelBase
{
    public required Guid VolunteerId { get; set; }
    public required Guid AnimalId { get; set; }
    public required DateTime Time { get; set; }
    public Status Status { get; set; } = Status.Pending;
}