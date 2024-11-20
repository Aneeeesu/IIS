using AutoMapper;
using IISBackend.BL.Models.Interfaces;
using IISBackend.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.BL.Models.Schedules;

public record ScheduleBaseModel : IModel
{
    public Guid Id { get; init; }
    public required DateTime Time { get; set; }
    public ScheduleType Type { get; set; }
}
