using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.User;

public record UserUpdateModel : UserBaseModel
{
    public ICollection<string>? Roles { get; set; }
    public Guid? ImageId { get; set; }
}
