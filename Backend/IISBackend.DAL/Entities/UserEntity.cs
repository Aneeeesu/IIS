using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public class UserEntity : IdentityUser<Guid>, IEntity
{
    public ICollection<ReservationRequestEntity>? ReservationRequests { get; set; }
    public ICollection<ScheduleEntryEntity>? ScheduleEntries { get; set; }
    public VerificationRequest? VerificationRequest { get; set; }
    public Guid? ImageId { get; set; }
    [ForeignKey(nameof(ImageId)),]
    public FileEntity? Image { get; set; }

    public ICollection<FileEntity>? Files { get; set; }
}

public class UserEntityMapperProfile : Profile
{
    public UserEntityMapperProfile()
    {
        CreateMap<UserEntity, UserEntity>();
    }
}