using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Animal;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using IISBackend.Common.Enums;
using Microsoft.IdentityModel.Tokens;
using Google.Protobuf.WellKnownTypes;

namespace IISBackend.BL.Facades;


public class AnimalFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper) : FacadeCRUDBase<AnimalEntity,AnimalCreateModel, AnimalListModel, Models.Animal.AnimalDetailModel>(unitOfWorkFactory, modelMapper), IAnimalFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { $"{nameof(AnimalEntity.Image)}" };

    public override async Task<AnimalDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        IQueryable<AnimalEntity> query = uow.GetRepository<AnimalEntity>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        AnimalEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        var statusRepo = uow.GetRepository<AnimalStatusEntity>();
        var lastRecordTime =statusRepo.Get().IsNullOrEmpty() ? DateTime.MinValue : statusRepo.Get().Max(x => x.TimeStamp);

        return entity is null
            ? null
            : _modelMapper.Map<Models.Animal.AnimalDetailModel>(entity) with { LastStatus = (await uow.GetRepository<AnimalStatusEntity>().Get().FirstOrDefaultAsync(x => x.TimeStamp == lastRecordTime))?.Status ?? AnimalStatus.Available };
    }

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
