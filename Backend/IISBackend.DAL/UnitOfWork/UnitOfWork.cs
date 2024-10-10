using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IISBackend.DAL.UnitOfWork;

public sealed class UnitOfWork(DbContext dbContext) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public IRepository<TEntity> GetRepository<TEntity>(IMapper mapper)
        where TEntity : class, IEntity
        => new Repository<TEntity>(_dbContext, mapper);

    public async Task CommitAsync() => await _dbContext.SaveChangesAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync().ConfigureAwait(false);
}
