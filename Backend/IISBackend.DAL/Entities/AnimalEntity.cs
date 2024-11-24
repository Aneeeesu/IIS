// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record AnimalEntity : IEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Sex sex { get; set; }

    public AnimalStatus Status { get; set; }

    public Guid? ImageId { get; set; }
    [ForeignKey(nameof(ImageId))]
    public FileEntity? Image { get; set; }
    public ICollection<ReservationRequestEntity>? ReservationRequests { get; set; }
    public ICollection<ScheduleEntryEntity>? ScheduleEntries { get; set; }
    public ICollection<HealthRecordEntity>? HealthRecords { get; set; }
    public ICollection<AnimalStatusEntity>? AnimalStatusRecords { get; set; }

}
partial class FileEntity
{
    public ICollection<AnimalEntity>? AnimalImages { get; }
}

public class AnimalEntityMapperProfile : Profile
{
    public AnimalEntityMapperProfile()
    {
        CreateMap<AnimalEntity, AnimalEntity>();
    }
}
