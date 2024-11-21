using AutoMapper;
using IISBackend.BL.Models.File;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class FileMapperProfile : Profile
{
    public FileMapperProfile()
    {
        CreateMap<FileEntity,FileBaseModel>();
        CreateMap<FileEntity, FileDetailModel>();
    }
}