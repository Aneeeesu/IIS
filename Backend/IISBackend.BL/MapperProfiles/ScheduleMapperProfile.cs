using AutoMapper;
using IISBackend.BL.Models.Schedules;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class ScheduleMapperProfile : Profile
{
    public ScheduleMapperProfile()
    {
        CreateMap<ScheduleEntryEntity, ScheduleListModel>();
        CreateMap<ScheduleEntryEntity, ScheduleDetailModel>();

        CreateMap<ScheduleCreateModel, ScheduleEntryEntity>();
    }
}