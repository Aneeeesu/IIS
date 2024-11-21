using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.User;

namespace IISBackend.BL.Models.File;

public class FileBaseModel
{
    public required Guid Id { get; set; }
    public required string Url { get; set; }
}

public class FileDetailModel : FileBaseModel
{
    public required ICollection<UserListModel> UserImages { get; set; }
    public required ICollection<AnimalListModel> AnimalImages { get; set; }
}