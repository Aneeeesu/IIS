using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record ReservationRequestEntity : IEntity, IUserAuthorized
{
    public Guid Id { get; set; }
    public required UserEntity Voluteer { get; set; }
    public required AnimalEntity Animal { get; set; }
    public required DateTime Time { get; set; }
    public required ScheduleType Type { get; set; }

    public Guid GetOwnerID() => Voluteer.Id;
}

public class ReservationRequestEntityMapperProfile : Profile
{
    public ReservationRequestEntityMapperProfile()
    {
        CreateMap<ReservationRequestEntity, ReservationRequestEntity>();
    }
}