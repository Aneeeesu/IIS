namespace IISBackend.BL.Models.User;

public record UserDetailModel : UserBaseModel
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}
