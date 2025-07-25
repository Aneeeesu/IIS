﻿using IISBackend.BL.Models.Interfaces;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Models.Animal;

public class AnimalStatusBaseModel: IModel
{
    public Guid Id { get; init; }
    public required Guid AnimalId { get; set; }
    public required Guid AssociatedUserId { get; set; }
    public required AnimalStatus Status { get; set; }
}
