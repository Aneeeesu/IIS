using AutoMapper;
using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Validators;
using IISBackend.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.BL.Models.Schedules;

public record ScheduleBaseModel : IModel
{
    public Guid Id { get; init; }
    [DateIsInFuture(ErrorMessage = "Date must be in the future")]
    [RoundedToHour(ErrorMessage ="Time must be rounded to hours")]
    public required DateTime Time { get; set; }
    public ScheduleType Type { get; set; }
}
