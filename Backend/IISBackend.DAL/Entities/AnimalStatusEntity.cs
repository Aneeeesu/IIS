// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AutoMapper;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public record AnimalStatusEntity : IEntity
{
    public required Guid Id { get; set; }
    public required Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public AnimalEntity? Animal { get; set; }

    public required Guid AssociatedUserId { get; set; }
    [ForeignKey(nameof(AssociatedUserId))]
    public UserEntity? AssociatedUser { get; set; }

    public required DateTime TimeStamp { get; set; }
    public required AnimalStatus Status { get; set; }
}

public class AnimalStatusRecordMapperProfile : Profile
{
    public AnimalStatusRecordMapperProfile()
    {
        CreateMap<AnimalStatusEntity, AnimalStatusEntity>();
    }
}