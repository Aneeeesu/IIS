using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record ScheduleEntryEntity : IEntity,IUserAuthorized
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public UserEntity? User { get; }

    public required Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public AnimalEntity? Animal { get; }
    public required DateTime Time { get; set; }
    public required ScheduleType Type { get; set; }
    public Guid GetOwnerID() => UserId ?? Guid.Empty;

    public ICollection<ReservationRequestEntity>? WalkRequests { get; set; }
}

public class ScheduleEntryEntityMapperProfile : Profile
{
    public ScheduleEntryEntityMapperProfile()
    {
        CreateMap<ScheduleEntryEntity, ScheduleEntryEntity>();
    }
}
