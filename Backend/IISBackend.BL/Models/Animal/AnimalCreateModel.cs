using IISBackend.BL.Validators;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.Animal;

public record AnimalCreateModel : AnimalBaseModel
{
    public required string Name { get; set; }
    public Guid? ImageID { get; set; }
    [DateIsInPast]
    public DateTime DateOfBirth { get; set; }
    public Sex sex { get; set; }
}
