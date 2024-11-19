using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.User;

public record UserUpdateModel : UserBaseModel
{
    public required string Email { get; set; }
    public ICollection<string>? Roles { get; set; }
}
