namespace IISBackend.BL.Models.Requests;

public record ReservationRequestCreateModel : ReservationRequestBaseModel
{
    public required Guid TargetUserId { get; set; }
    public required Guid AnimalId { get; set; }
}