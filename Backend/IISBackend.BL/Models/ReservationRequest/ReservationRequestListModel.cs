using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.ReservationRequest;

public record ReservationRequestListModel : ModelBase
{
    public Guid Id { get; set; }
    public Guid VolunteerId { get; set; }
    public Guid AnimalId { get; set; }
    public DateTime Time { get; set; }
    public Status Status { get; set; }
}
