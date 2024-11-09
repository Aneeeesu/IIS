using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using ITUBackend.API.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record ScheduleEntryEntity : IEntity,IUserAuthorized
{
    public Guid Id { get; set; }
    public required Guid VolunteerId { get; set; }
    [ForeignKey(nameof(VolunteerId))]
    public UserEntity? Volunteer { get; set; }

    public required Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public AnimalEntity? Animal { get; set; }
    public required DateTime Time { get; set; }
    public Guid GetOwnerID() => VolunteerId;
}

public class ScheduleEntryEntityMapperProfile : Profile
{
    public ScheduleEntryEntityMapperProfile()
    {
        CreateMap<ScheduleEntryEntity, ScheduleEntryEntity>();
    }
}
