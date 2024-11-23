using IISBackend.BL.Models.File;

namespace IISBackend.BL.Models.Animal;

public record AnimalListModel : AnimalBaseModel
{
    public required string Name { get; set; }
    public required FileBaseModel Image { get; set; }
}
