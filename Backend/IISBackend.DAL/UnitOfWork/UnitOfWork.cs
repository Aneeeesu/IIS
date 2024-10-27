using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IISBackend.DAL.UnitOfWork;

public sealed class UnitOfWork(DbContext dbContext,UserManager<UserEntity> userManager, IMapper mapper) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly UserManager<UserEntity> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly RoleManager<RoleEntity> _roleManager;

    public IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity
        => new Repository<TEntity>(_dbContext, mapper);



    public UserManager<UserEntity> GetUserManager() => _userManager;
    public RoleManager<RoleEntity> GetRoleManager() => _roleManager;

    public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync().ConfigureAwait(false);
    public async Task CommitAsync() => await _dbContext.SaveChangesAsync().ConfigureAwait(false);
}
