using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.Requests;

public record VerificationRequestListModel : VerificationRequestBaseModel
{
    public required UserListModel Requestee { get; set; }
}
