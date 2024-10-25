using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record VerificationRequest : IEntity
{
    public Guid Id { get; set; }
    public UserEntity Requestee { get; set; }

    public string Content { get; set; }
}