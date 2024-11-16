using AutoMapper;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Entities.Interfaces;
using IISBackend.DAL.Extensions;
using IISBackend.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.DAL.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ProjectDbContext _dbContext;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly IMapper _mapper;

    public UnitOfWork(ProjectDbContext dbContext, UserManager<UserEntity> userManager,SignInManager<UserEntity> signInManager,IMapper mapper)
    {
        if (dbContext == null)
            throw new ArgumentNullException(nameof(dbContext));
        if (userManager == null)
            throw new ArgumentNullException(nameof(userManager));
        if (signInManager == null)
            throw new ArgumentNullException(nameof(signInManager));
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity
        => new Repository<TEntity>(_dbContext, _mapper);



    public UserManager<UserEntity> GetUserManager() => _userManager;

    public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync().ConfigureAwait(false);
    public async Task CommitAsync() => await _dbContext.SaveChangesAsync().ConfigureAwait(false);

    public SignInManager<UserEntity> GetSignInManager() => _signInManager;
}