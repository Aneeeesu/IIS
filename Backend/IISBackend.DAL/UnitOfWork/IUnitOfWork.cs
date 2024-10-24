using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity>(IMapper mapper)
        where TEntity : class, IEntity;

    UserManager<TUser> GetUserManager<TUser>() where TUser : IdentityUser<Guid>;
    Task CommitAsync();
}
