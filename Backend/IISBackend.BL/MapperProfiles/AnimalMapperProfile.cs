using AutoMapper;
using IISBackend.BL.Models.Animal;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class AnimalMapperProfile : Profile
{
    public AnimalMapperProfile()
    {
        CreateMap<AnimalEntity, AnimalListModel>();
        CreateMap<AnimalEntity, AnimalDetailModel>();

        CreateMap<AnimalCreateModel, AnimalEntity>();
    }
}
