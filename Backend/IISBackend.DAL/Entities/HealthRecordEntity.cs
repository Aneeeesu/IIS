using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record HealthRecordEntity : IEntity
{
    public Guid Id { get; set; }
    public required UserEntity Vet { get; set; }
    public required AnimalEntity Animal { get; set; }
    public required DateTime Time { get; set; }
    public required string Content { get; set; }
}


public class HealthRecordMapperProfile : Profile
{
    public HealthRecordMapperProfile()
    {
        CreateMap<HealthRecordEntity, HealthRecordEntity>();
    }
}