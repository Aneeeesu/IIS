using IISBackend.BL.Models.File;

namespace IISBackend.BL.Models.User;

public record UserListModel : UserBaseModel {
    public required string UserName { get; set; }
    public required ICollection<string> Roles { get; set; }
    public FileBaseModel? Image { get; set; }
}