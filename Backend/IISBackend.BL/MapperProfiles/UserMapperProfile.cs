using AutoMapper;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, UserListModel>();
        CreateMap<UserEntity, UserDetailModel>();

        CreateMap<UserCreateModel, UserEntity>();
    }
}