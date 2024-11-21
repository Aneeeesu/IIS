using IISBackend.BL.Models.File;

namespace IISBackend.BL.Models.Animal;

public record AnimalListModel : ModelBase
{
    public required string Name { get; set; }
    public required FileBaseModel Image { get; set; }
}
