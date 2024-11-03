using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record ReservationRequestEntity : IEntity
{
    public Guid Id { get; set; }
    public required UserEntity Volunteer { get; set; }
    public required AnimalEntity Animal { get; set; }
    public required DateTime Time { get; set; }
    public required Status status { get; set; }
}

public class ReservationRequestEntityMapperProfile : Profile
{
    public ReservationRequestEntityMapperProfile()
    {
        CreateMap<ReservationRequestEntity, ReservationRequestEntity>();
    }
}