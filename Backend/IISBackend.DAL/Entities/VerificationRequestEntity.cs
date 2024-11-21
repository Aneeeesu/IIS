using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record VerificationRequestEntity : IEntity, IUserAuthorized
{
    public Guid Id { get; set; }

    public required Guid RequesteeID { get; set; }
    [ForeignKey("RequesteeID")]
    public UserEntity? Requestee { get; set; }
    public required string Content { get; set; }
    public Guid GetOwnerID() => RequesteeID;
}

public class VerificationRequestMapperProfile : Profile
{
    public VerificationRequestMapperProfile()
    {
        CreateMap<VerificationRequestEntity, VerificationRequestEntity>();
    }
}