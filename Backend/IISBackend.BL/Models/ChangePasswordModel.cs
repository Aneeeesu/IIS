namespace IISBackend.BL.Models;

public record ChangePasswordModel
{
    public required Guid UserId { get; set; }
    public string? OldPassword { get; set; }
    public required string NewPassword { get; set; }
}