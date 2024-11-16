using AutoMapper;
using IISBackend.DAL.Authorization;
using IISBackend.DAL.Entities;
using IISBackend.DAL.Extensions;
using IISBackend.DAL.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IISBackend.DAL.UnitOfWork;

public class UnitOfWorkFactory(ProjectDbContext dbContext,UserManager<UserEntity> userManager, IMapper modelMapper, IServiceProvider provider,DALOptions options) : IUnitOfWorkFactory
{
    public IUnitOfWork Create(){
        var newDbContext = dbContext.Clone(options);
        var newUserManager = userManager.Clone(newDbContext, provider);
        var signInManager = newUserManager.CreateSignInManager(provider);
        return new UnitOfWork(newDbContext, newUserManager, signInManager, modelMapper);
    }

    public ITransactionalUnitOfWork CreateTransactional()
    {
        var newDbContext = dbContext.Clone(options);
        var newUserManager = userManager.Clone(newDbContext, provider);
        var signInManager = newUserManager.CreateSignInManager(provider);
        return new TransactionalUnitOfWork(newDbContext, newUserManager, signInManager, modelMapper);
    }
}