using IISBackend.Common.Enums;

namespace IISBackend.BL.Models;

public record AnimalCreateModel : ModelBase
{
    public required string Name { get; set; }
    public int age { get; set; }
    public Sex sex { get; set; }
}
