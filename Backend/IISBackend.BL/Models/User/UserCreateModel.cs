using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.User;

public record UserCreateModel : UserBaseModel
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
