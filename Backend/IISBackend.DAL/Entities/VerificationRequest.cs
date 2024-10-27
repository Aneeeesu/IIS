using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record VerificationRequest : IEntity
{
    public Guid Id { get; set; }

    public required Guid RequesteeID { get; set; }
    [ForeignKey("RequesteeID")]
    public UserEntity? Requestee { get; set; }
    public required string Content { get; set; }
}