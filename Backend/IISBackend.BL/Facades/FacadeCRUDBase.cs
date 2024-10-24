using Microsoft.EntityFrameworkCore;
using AutoMapper;
using IISBackend.DAL.Repositories;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Facades.Interfaces;
using System.Security.Claims;
using IISBackend.BL.Models.Interfaces;

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
    protected readonly IMapper modelMapper = modelMapper;
    protected readonly IUnitOfWorkFactory UOWFactory = unitOfWorkFactory;

    protected virtual ICollection<string> IncludesNavigationPathDetail => new List<string>();

    public async Task DeleteAsync(Guid id)
    {
        await using IUnitOfWork uow = UOWFactory.Create();
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
        await using IUnitOfWork uow = UOWFactory.Create();

        IQueryable<TEntity> query = uow.GetRepository<TEntity>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

        return entity is null
            ? null
            : modelMapper.Map<TDetailModel>(entity);
    }

    // Always use paging in production
    public virtual async Task<IEnumerable<TListModel>> GetAsync()
    {
        await using IUnitOfWork uow = UOWFactory.Create();
        List<TEntity> entities = await uow
            .GetRepository<TEntity>()
            .Get()
            .ToListAsync().ConfigureAwait(false);

        return modelMapper.Map<List<TListModel>>(entities);
    }

    public virtual async Task<TDetailModel?> SaveAsync(TCreateModel model, ClaimsPrincipal? userPrincipal = null)
    {
        TDetailModel result;

        TEntity entity = modelMapper.Map<TEntity>(model);

        IUnitOfWork uow = UOWFactory.Create();
        IRepository<TEntity> repository = uow.GetRepository<TEntity>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            TEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = modelMapper.Map<TDetailModel>(updatedEntity);
        }
        else
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            TEntity insertedEntity = repository.Insert(entity);
            result = modelMapper.Map<TDetailModel>(insertedEntity);
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
