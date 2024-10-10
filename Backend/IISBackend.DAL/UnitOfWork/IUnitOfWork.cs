using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Repositories;

namespace IISBackend.DAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity>(IMapper mapper)
        where TEntity : class, IEntity;

    Task CommitAsync();
}
