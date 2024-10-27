using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Entities;

public class RoleEntity : IdentityRole<Guid>, IEntity
{
}
public class RoleEntityMapperProfile : Profile
{
    public RoleEntityMapperProfile()
    {
        CreateMap<RoleEntity, RoleEntity>();
    }
}