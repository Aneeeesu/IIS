namespace IISBackend.BL.Models.Requests;

public class ReservationRequestCreateModel : ReservationRequestBaseModel
{
    public required Guid TargetUserId { get; set; }
    public required Guid AnimalId { get; set; }
}