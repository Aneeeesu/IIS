using IISBackend.BL.Models.Interfaces;

namespace IISBackend.BL.Models.Animal;

public record AnimalBaseModel : IModel
{
    public required Guid Id { get; init; }
}