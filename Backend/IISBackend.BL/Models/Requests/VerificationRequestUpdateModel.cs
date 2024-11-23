using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.Requests;

public record VerificationRequestDetailModel : VerificationRequestBaseModel
{
    public required UserListModel Requestee { get; set; }
    public required string Content { get; set; }
}