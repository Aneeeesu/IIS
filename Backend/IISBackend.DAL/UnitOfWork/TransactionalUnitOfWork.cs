using AutoMapper;
using IISBackend.DAL.Authorization;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace IISBackend.DAL.UnitOfWork;

public sealed class TransactionalUnitOfWork : ITransactionalUnitOfWork
{
    //using var transaction;
    public TransactionalUnitOfWork(DbContext dbContext, UserManager<UserEntity> userManager, IMapper mapper)
    {
        _transaction = dbContext.Database.BeginTransaction();
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper;
    }

    IDbContextTransaction _transaction;

    private readonly DbContext _dbContext;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMapper _mapper;

    public IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity
        => new Repository<TEntity>(_dbContext, _mapper);



    public UserManager<UserEntity> GetUserManager() => _userManager;

    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    public async Task RevertChangesAsync() => await _transaction.RollbackAsync().ConfigureAwait(false);
    public async ValueTask DisposeAsync() => await _transaction.DisposeAsync().ConfigureAwait(false);
    public async Task CommitAsync() => await _transaction.CommitAsync().ConfigureAwait(false);
}