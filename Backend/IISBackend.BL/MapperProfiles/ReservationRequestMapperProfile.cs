using AutoMapper;
using IISBackend.BL.Models.Requests;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class ReservationRequestMapperProfile : Profile
{
    public ReservationRequestMapperProfile()
    {
        CreateMap<ReservationRequestEntity, ReservationRequestListModel>();
        CreateMap<ReservationRequestEntity, ReservationRequestDetailModel>();

        CreateMap<ReservationRequestCreateModel, ReservationRequestEntity>();
    }
}