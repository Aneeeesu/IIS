using IISBackend.BL.Models.Interfaces;

namespace IISBackend.BL.Models.User;

public record UserBaseModel : IModel
{
    public Guid Id { get; init; }
}