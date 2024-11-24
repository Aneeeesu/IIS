using IISBackend.BL.Models.Interfaces;

namespace IISBackend.BL.Models.Animal;

public record AnimalBaseModel : IModel
{
    public Guid Id { get; init; }
}