using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.HealthRecords;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace IISBackend.BL.Facades;

public class HealthRecordFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper) : FacadeCRUDBase<HealthRecordEntity,HealthRecordCreateModel,HealthRecordListModel,HealthRecordDetailModel>(unitOfWorkFactory,modelMapper), IHealthRecordFacade
{
    private readonly IUnitOfWorkFactory _uowFactory = unitOfWorkFactory;
    protected override ICollection<string> IncludesNavigationPathDetail => 
        new[] { $"{nameof(HealthRecordEntity.Animal)}", $"{nameof(HealthRecordDetailModel.Vet)}"  };

    public async Task<HealthRecordDetailModel> AuthorizedCreateAsync(HealthRecordCreateModel record, ClaimsPrincipal user)
    {
        await using var uow = _uowFactory.Create();
        if(await uow.GetUserManager().GetUserAsync(user) is not { } creator)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        if(creator.Id != record.VetId && await uow.GetUserManager().IsInRoleAsync(creator,"Admin") is not true)
        {
            throw new UnauthorizedAccessException("User is not authorized to create health record for another vet");
        }
        if (await uow.GetRepository<AnimalEntity>().Get().FirstOrDefaultAsync(o => o.Id == record.AnimalId) is not { } animal)
        {
            throw new ArgumentException("Animal not found");
        }
        if (await uow.GetRepository<UserEntity>().Get().FirstOrDefaultAsync(o => o.Id == record.VetId) is not { } vet)
        {
            throw new ArgumentException("Vet not found");
        }
        return await base.CreateAsync(record);
    }

    public async Task<ICollection<HealthRecordListModel>> GetAnimalRecords(Guid animalID)
    {
        await using var uow = _uowFactory.Create();
        if(await uow.GetRepository<AnimalEntity>().Get().FirstOrDefaultAsync(o => o.Id == animalID) is null)
        {
            throw new ArgumentException("Animal not found");
        }
        return await uow.GetRepository<HealthRecordEntity>().Get().Where(o => o.AnimalId == animalID).Select(o => _modelMapper.Map<HealthRecordListModel>(o)).ToListAsync();
    }
}