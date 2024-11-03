using IISBackend.BL.Models.ReservationRequest;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IReservationRequestFacade
        : IFacadeCRUD<ReservationRequestEntity, ReservationRequestCreateModel, ReservationRequestListModel, ReservationRequestDetailModel>
    {
        Task ApproveRequestAsync(Guid requestId);
        Task RejectRequestAsync(Guid requestId);
    }
}