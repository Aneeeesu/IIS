using AutoMapper;
using IISBackend.BL.Models.Animal;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class AnimalStatusMapperProfile : Profile
{
    public AnimalStatusMapperProfile()
    {
        CreateMap<AnimalStatusEntity, AnimalStatusListModel>();
        CreateMap<AnimalStatusEntity, AnimalStatusDetailModel>();

        CreateMap<AnimalStatusCreateModel, AnimalStatusEntity>().ForMember(dest => dest.TimeStamp,o=>o.MapFrom(src=>DateTime.Now));
    }
}
