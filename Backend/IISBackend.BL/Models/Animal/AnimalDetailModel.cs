using IISBackend.BL.Models.File;
using IISBackend.BL.Validators;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.Animal;

public record AnimalDetailModel : AnimalBaseModel
{
    public required string Name { get; set; }
    public required FileBaseModel Image { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Sex sex { get; set; }
    public required AnimalStatus LastStatus { get; set; } = AnimalStatus.Available;
}

