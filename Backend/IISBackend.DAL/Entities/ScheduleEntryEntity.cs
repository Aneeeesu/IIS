using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record ScheduleEntryEntity : IEntity
{
    public Guid Id { get; set; }
    public required UserEntity Volunteer { get; set; }
    public required AnimalEntity Animal { get; set; }
}
