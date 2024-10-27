// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Entities;

public record AnimalEntity : IEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public int age { get; set; }
    public Sex sex { get; set; }
    public ICollection<ReservationRequestEntity>? ReservationRequests { get; set; }
    public ICollection<ScheduleEntryEntity>? ScheduleEntries { get; set; }
    public ICollection<HealthRecordEntity>? HealthRecords { get; set; }
}

public class AnimalEntityMapperProfile : Profile
{
    public AnimalEntityMapperProfile()
    {
        CreateMap<AnimalEntity, AnimalEntity>();
    }
}
