using IISBackend.BL.Models.File;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.Animal;

public record AnimalDetailModel : ModelBase
{
    public required string Name { get; set; }
    public required FileBaseModel Image { get; set; }
    public int age { get; set; }
    public Sex sex { get; set; }
}

