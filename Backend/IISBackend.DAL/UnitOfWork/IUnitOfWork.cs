using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity;

    UserManager<UserEntity> GetUserManager();
    RoleManager<IdentityRole<Guid>> GetRoleManager();
    Task CommitAsync();
}
