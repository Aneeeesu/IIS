using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record ReservationRequestEntity : IEntity, IUserAuthorized
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public UserEntity? User { get; }
    public required Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public AnimalEntity? Animal { get; }
    public required DateTime Time { get; set; }
    public required ScheduleType Type { get; set; }
    public Guid? TargetScheduleId { get; set; }
    [ForeignKey(nameof(TargetScheduleId))]
    public ScheduleEntryEntity? TargetSchedule { get; }
    public Guid GetOwnerID() => User!.Id;
}

public class ReservationRequestEntityMapperProfile : Profile
{
    public ReservationRequestEntityMapperProfile()
    {
        CreateMap<ReservationRequestEntity, ReservationRequestEntity>();
        CreateMap<ReservationRequestEntity, ScheduleEntryEntity>();
    }
}