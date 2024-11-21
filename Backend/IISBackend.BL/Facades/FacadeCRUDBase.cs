using Microsoft.EntityFrameworkCore;
using AutoMapper;
using IISBackend.DAL.Repositories;
using IISBackend.DAL.UnitOfWork;
using IISBackend.BL.Facades.Interfaces;
using System.Security.Claims;
using IISBackend.BL.Models.Interfaces;
using IISBackend.DAL.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IISBackend.BL.Facades;

public abstract class FacadeCRUDBase<TEntity,TCreateModel, TListModel, TDetailModel>(
        IUnitOfWorkFactory unitOfWorkFactory,
        IMapper modelMapper)

    : IFacadeCRUD<TEntity,TCreateModel, TListModel, TDetailModel>
    where TEntity : class, IEntity
    where TCreateModel : class, IModel
    where TListModel : class, IModel
    where TDetailModel : class, IModel
{
    protected readonly IMapper _modelMapper = modelMapper;
    protected readonly IUnitOfWorkFactory _UOWFactory = unitOfWorkFactory;

    protected virtual ICollection<string> IncludesNavigationPathDetail => new List<string>();

    public async Task DeleteAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();
        try
        {
            await uow.GetRepository<TEntity>().DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("zabiju se tzv.", e);
        }
    }

    public virtual async Task<TDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = _UOWFactory.Create();

        IQueryable<TEntity> query = uow.GetRepository<TEntity>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

        return entity is null
            ? null
            : _modelMapper.Map<TDetailModel>(entity);
    }

    // Always use paging in production
    public virtual async Task<IEnumerable<TListModel>> GetAsync()
    {
        await using IUnitOfWork uow = _UOWFactory.Create();
        IQueryable<TEntity> query = uow.GetRepository<TEntity>().Get();
        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }
        var entities = await query.ToListAsync().ConfigureAwait(false);

        return _modelMapper.Map<List<TListModel>>(entities);
    }


    public virtual async Task<TDetailModel?> CreateAsync(TCreateModel model)
    {
        TEntity entity = _modelMapper.Map<TEntity>(model);

        await using IUnitOfWork uow = _UOWFactory.Create();
        entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        TEntity insertedEntity = await uow.GetRepository<TEntity>().InsertAsync(entity);

        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        return _modelMapper.Map<TDetailModel>(insertedEntity);
    }

    public virtual async Task<TDetailModel?> UpdateAsync(TCreateModel model)
    {
        TEntity entity = _modelMapper.Map<TEntity>(model);

        await using IUnitOfWork uow = _UOWFactory.Create();

        TEntity updatedEntity = await uow.GetRepository<TEntity>().UpdateAsync(entity).ConfigureAwait(false);

        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        return _modelMapper.Map<TDetailModel>(updatedEntity);
    }

    public virtual async Task<TDetailModel?> SaveAsync(TCreateModel model)
    {
        TDetailModel result;

        TEntity entity = _modelMapper.Map<TEntity>(model);

        await using IUnitOfWork uow = _UOWFactory.Create();
        IRepository<TEntity> repository = uow.GetRepository<TEntity>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            TEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = _modelMapper.Map<TDetailModel>(updatedEntity);
        }
        else
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            TEntity insertedEntity = await repository.InsertAsync(entity);
            result = _modelMapper.Map<TDetailModel>(insertedEntity);
        }
        try
        {
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        return result;
    }
}
