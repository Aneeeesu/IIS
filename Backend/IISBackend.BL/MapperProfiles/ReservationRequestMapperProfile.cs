using AutoMapper;
using IISBackend.BL.Models.ReservationRequest;
using IISBackend.DAL.Entities;

public class ReservationRequestMapperProfile : Profile
{
    public ReservationRequestMapperProfile()
    {
        CreateMap<ReservationRequestEntity, ReservationRequestDetailModel>()
            .ForMember(dest => dest.VolunteerId, opt => opt.MapFrom(src => src.Volunteer.Id))
            .ForMember(dest => dest.AnimalId, opt => opt.MapFrom(src => src.Animal.Id));

        CreateMap<ReservationRequestCreateModel, ReservationRequestEntity>();
    }
}