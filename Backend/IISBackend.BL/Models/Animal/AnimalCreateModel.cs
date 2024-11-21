using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.Animal;

public record AnimalCreateModel : ModelBase
{
    public required string Name { get; set; }
    public Guid? ImageID { get; set; }
    public int age { get; set; }
    public Sex sex { get; set; }
}
