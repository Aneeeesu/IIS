using AutoMapper;
using IISBackend.BL.Models.File;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class PendingUploadMapperProfile : Profile
{
    public PendingUploadMapperProfile()
    {
        CreateMap<PendingFileUploadEntity, PendingFileUploadModel>();
    }
}
