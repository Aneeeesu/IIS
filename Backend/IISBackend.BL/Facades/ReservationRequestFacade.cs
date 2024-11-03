using AutoMapper;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.ReservationRequest;
using IISBackend.DAL.Entities;
using IISBackend.DAL.UnitOfWork;
using IISBackend.Common.Enums;

namespace IISBackend.BL.Facades
{
    public class ReservationRequestFacade(
        IUnitOfWorkFactory unitOfWorkFactory,
        IMapper modelMapper)
        : FacadeCRUDBase<ReservationRequestEntity, ReservationRequestCreateModel, ReservationRequestListModel, ReservationRequestDetailModel>(unitOfWorkFactory, modelMapper),
          IReservationRequestFacade
    {
        public async Task ApproveRequestAsync(Guid requestId)
        {
            var request = await GetAsync(requestId);
            if (request != null)
            {
                await using var uow = UOWFactory.Create();
                var repository = uow.GetRepository<ReservationRequestEntity>();
                var entity = modelMapper.Map<ReservationRequestEntity>(request);
                entity.status = Status.Approved;
                repository.UpdateAsync(entity);
                await uow.CommitAsync();
            }
        }

        public async Task RejectRequestAsync(Guid requestId)
        {
            var request = await GetAsync(requestId);
            if (request != null)
            {
                await using var uow = UOWFactory.Create();
                var repository = uow.GetRepository<ReservationRequestEntity>();
                var entity = modelMapper.Map<ReservationRequestEntity>(request);
                entity.status = Status.Rejected;
                repository.UpdateAsync(entity);
                await uow.CommitAsync();
            }
        }
    }
}