using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using IISBackend.BL.Models.Animal;

namespace IISBackend.BL.Facades;

public class AnimalStatusFacade : FacadeCRUDBase<AnimalStatusEntity, AnimalStatusCreateModel, AnimalStatusListModel, AnimalStatusDetailModel>, IAnimalStatusFacade
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _modelMapper;
    public AnimalStatusFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper) : base(unitOfWorkFactory, modelMapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _modelMapper = modelMapper;
    }

    public override async Task<AnimalStatusDetailModel> CreateAsync(AnimalStatusCreateModel model)
    {
        AnimalStatusEntity entity = _modelMapper.Map<AnimalStatusEntity>(model);
        await using IUnitOfWork uow = _unitOfWorkFactory.Create();

        if(uow.GetUserManager().FindByIdAsync(model.AssociatedUserId.ToString()).Result is null)
        {
            throw new ArgumentException("User not found");
        }
        if(uow.GetRepository<AnimalEntity>().Get().FirstOrDefaultAsync(o => o.Id == model.AnimalId).Result is null)
        {
            throw new ArgumentException("Animal not found");
        }
        if(uow.GetRepository<AnimalStatusEntity>().Get().FirstOrDefaultAsync(o=>o.Id == model.Id).Result is not null)
        {
            throw new ArgumentException("Status id is already taken");
        }

        IRepository<AnimalStatusEntity> repository = uow.GetRepository<AnimalStatusEntity>();
        AnimalStatusEntity insertedEntity = await repository.InsertAsync(entity);
        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
            return _modelMapper.Map<AnimalStatusDetailModel>(insertedEntity);
        }
        catch
        {
            throw new InvalidOperationException("Error while saving entity");
        }
    }
}
