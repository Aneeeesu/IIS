using IISBackend.BL.Models.File;
using IISBackend.BL.Models.Roles;

namespace IISBackend.BL.Models.User;

public record UserDetailModel : UserBaseModel
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required ICollection<string> Roles { get; set; }
    public FileModel? Image { get; set; }
}
