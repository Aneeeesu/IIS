using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Animal;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace IISBackend.BL.Facades;


public class AnimalFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper) : FacadeCRUDBase<AnimalEntity,AnimalCreateModel, AnimalListModel, AnimalDetailModel>(unitOfWorkFactory, modelMapper), IAnimalFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { $"{nameof(AnimalEntity.Image)}" };

    public override async Task<AnimalDetailModel?> SaveAsync(AnimalCreateModel model)
    {
        AnimalDetailModel result;

        AnimalEntity entity = _modelMapper.Map<AnimalEntity>(model);
        await using IUnitOfWork uow = _UOWFactory.Create();

        var existingEntity = await uow.GetRepository<AnimalEntity>().Get().Include(x=>x.Image).FirstOrDefaultAsync(o=>o.Id == model.Id).ConfigureAwait(false);
        if (model.ImageID != null)
        {
            IRepository<FileEntity> fileRepository = uow.GetRepository<FileEntity>();
            FileEntity fileEntity = await fileRepository.Get().FirstOrDefaultAsync(o=>o.Id == model.ImageID).ConfigureAwait(false);
            if(fileEntity is null)
            {
                throw new ArgumentException("Image not found");
            }
            entity.Image = fileEntity;
            await fileRepository.UpdateAsync(fileEntity).ConfigureAwait(false);
        }


        IRepository<AnimalEntity> repository = uow.GetRepository<AnimalEntity>();

        if (existingEntity is not null)
        {
            AnimalEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = _modelMapper.Map<AnimalDetailModel>(updatedEntity);
        }
        else
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            AnimalEntity insertedEntity = await repository.InsertAsync(entity);
            result = _modelMapper.Map<AnimalDetailModel>(insertedEntity);
        }
        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            throw new InvalidOperationException("Error while saving entity");
        }

        return result;
    }
}
