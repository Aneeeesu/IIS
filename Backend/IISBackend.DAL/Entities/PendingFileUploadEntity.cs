using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public class PendingFileUploadEntity : IEntity, IUserAuthorized
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public required Guid UploaderId { get; set; }
    [ForeignKey(nameof(UploaderId))]
    public UserEntity? Uploader { get; set; }

    public required string Key { get; set; }

    public Guid GetOwnerID() => UploaderId;

    public class PendingFileUploadEntityMapperProfile : Profile
    {
        public PendingFileUploadEntityMapperProfile()
        {
            CreateMap<UserEntity, UserEntity>();
        }
    }
}
