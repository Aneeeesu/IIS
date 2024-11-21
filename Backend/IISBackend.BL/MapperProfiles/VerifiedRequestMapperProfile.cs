using AutoMapper;
using IISBackend.BL.Models.Requests;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class VerificationRequestMapperProfile : Profile
{
    public VerificationRequestMapperProfile()
    {
        CreateMap<VerificationRequestEntity, VerificationRequestListModel>();
        CreateMap<VerificationRequestEntity, VerificationRequestDetailModel>();

        CreateMap<VerificationRequestCreateModel, VerificationRequestEntity>();
    }
}