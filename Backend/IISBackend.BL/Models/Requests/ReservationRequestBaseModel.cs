using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Validators;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Models.Requests;

public record ReservationRequestBaseModel : IModel
{
    public Guid Id { get; init; }

    [DateIsInFuture(ErrorMessage = "Date must be in the future")]
    [RoundedToHour(ErrorMessage = "Time must be rounded to hours")]
    public required DateTime Time { get; set; }

    [RequestValidEnum(ErrorMessage = "Invalid request type")]
    public required ScheduleType Type { get; set; }
}
