using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.Requests;

public record VerificationRequestDetailModel : VerificationRequestBaseModel
{
    public required UserListModel Requestee { get; set; }
}