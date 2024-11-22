using AutoMapper;
using IISBackend.BL.Models.HealthRecords;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.MapperProfiles;

public class HealthRecordsMapperProfile : Profile
{
    public HealthRecordsMapperProfile()
    {
        CreateMap<HealthRecordEntity, HealthRecordListModel>();
        CreateMap<HealthRecordEntity, HealthRecordDetailModel>();

        CreateMap<HealthRecordCreateModel, HealthRecordEntity>();
    }
}