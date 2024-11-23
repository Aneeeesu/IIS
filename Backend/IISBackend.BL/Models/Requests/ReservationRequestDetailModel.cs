using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.Requests;

public record ReservationRequestDetailModel : ReservationRequestBaseModel
{
    public required UserListModel TargetUser { get; set; }
    public required AnimalListModel Animal { get; set; }
    public required UserListModel Creator { get; set; }
}
