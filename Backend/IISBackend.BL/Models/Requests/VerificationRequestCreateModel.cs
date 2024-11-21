namespace IISBackend.BL.Models.Requests;

public class VerificationRequestCreateModel : VerificationRequestBaseModel
{
    public required Guid RequesteeID { get; set; }
    public required string Content { get; set; }
}
