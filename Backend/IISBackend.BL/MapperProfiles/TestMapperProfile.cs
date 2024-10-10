using AutoMapper;
using IISBackend.BL.Models;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class TestMapperProfile : Profile
{
    public TestMapperProfile()
    {
        CreateMap<AnimalEntity, AnimalListModel>();
        CreateMap<AnimalEntity, AnimalDetailModel>();

        CreateMap<AnimalCreateModel, AnimalEntity>();
    }
}
