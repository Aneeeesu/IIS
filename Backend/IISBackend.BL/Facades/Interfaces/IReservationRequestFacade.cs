using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using System.Security.Claims;
using IISBackend.BL.Models.Schedules;
using IISBackend.BL.Models.Requests;

public interface IReservationRequestFacade : IFacadeCRUD<ReservationRequestEntity, ReservationRequestCreateModel,ReservationRequestListModel,ReservationRequestDetailModel>
{
    Task<ReservationRequestDetailModel?> AuthorizedCreateRequest(ReservationRequestCreateModel model, ClaimsPrincipal user);
    Task<ScheduleDetailModel?> AuthorizedResolveRequest(Guid id,bool approved, ClaimsPrincipal user);
    Task AuthorizedCancelRequest(Guid id, ClaimsPrincipal user);
}
