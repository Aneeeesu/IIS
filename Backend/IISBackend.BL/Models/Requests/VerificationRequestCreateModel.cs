namespace IISBackend.BL.Models.Requests;

public record VerificationRequestCreateModel : VerificationRequestBaseModel
{
    public required Guid RequesteeID { get; set; }
    public required string Content { get; set; }
}
