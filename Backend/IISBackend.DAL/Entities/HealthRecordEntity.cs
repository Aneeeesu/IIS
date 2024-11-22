using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record HealthRecordEntity : IEntity
{
    public Guid Id { get; set; }
    public Guid VetId { get; set; }
    [ForeignKey(nameof(VetId))]
    public UserEntity? Vet { get; set; }
    public Guid AnimalId {get;set;}
    [ForeignKey(nameof(AnimalId))]
    public AnimalEntity? Animal { get; set; }
    public required DateTime Time { get; set; }
    public required string Content { get; set; }
    public required HealthRecordType Type { get; set; }
}


public class HealthRecordMapperProfile : Profile
{
    public HealthRecordMapperProfile()
    {
        CreateMap<HealthRecordEntity, HealthRecordEntity>();
    }
}