﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities.Interfaces;

namespace IISBackend.DAL.Repositories;

public class Repository<TEntity>(
    DbContext dbContext,
    IMapper entityMapper)
    : IRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Get() => _dbSet.AsNoTracking();

    public async ValueTask<bool> ExistsAsync(TEntity entity)
        => await _dbSet.AnyAsync(e => e.Id == entity.Id).ConfigureAwait(false);

    public async Task<TEntity> InsertAsync(TEntity entity)
        => (await _dbSet.AddAsync(entity)).Entity;

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        TEntity existingEntity = await _dbSet.SingleAsync(e => e.Id == entity.Id).ConfigureAwait(false);
        entityMapper.Map(entity, existingEntity);
        return existingEntity;
    }

    public async Task DeleteAsync(Guid entityId)
        => _dbSet.Remove(await _dbSet.SingleAsync(i => i.Id == entityId).ConfigureAwait(false));
}
