namespace IISBackend.BL.Models.File;

public record PendingFileUploadModel
{
    public required Guid Id { get; set; }
    public required string Url { get; set; }
}
