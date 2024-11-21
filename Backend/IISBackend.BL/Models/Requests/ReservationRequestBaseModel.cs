using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Models.User;
using IISBackend.BL.Validators;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Models.Requests;

public class ReservationRequestBaseModel : IModel
{
    public Guid Id { get; init; }

    [DateIsInFuture(ErrorMessage = "Date must be in the future")]
    [RoundedToHour(ErrorMessage = "Time must be rounded to hours")]
    public required DateTime Time { get; set; }
}

public class ReservationRequestListModel : ReservationRequestBaseModel
{
    public required UserListModel User { get; set; }
    public required AnimalListModel Animal { get; set; }
    public required ScheduleType Type { get; set; }

}

public class ReservationRequestDetailModel : ReservationRequestBaseModel
{
    public required UserListModel User { get; set; }
    public required AnimalListModel Animal { get; set; }
    public required ScheduleType Type { get; set; }
}

public class ReservationRequestCreateModel : ReservationRequestBaseModel
{
    public required Guid UserID { get; set; }
    public required Guid AnimalID { get; set; }
    public required ScheduleType Type { get; set; }
}