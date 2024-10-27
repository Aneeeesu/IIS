using AutoMapper;
using IISBackend.DAL.Authorization;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IISBackend.DAL.UnitOfWork;

public class UnitOfWorkFactory(ProjectDbContext dbContext,UserManager<UserEntity> userManager, IMapper modelMapper) : IUnitOfWorkFactory
{
    public IUnitOfWork Create(){
        return new UnitOfWork(dbContext, userManager, modelMapper);
    }

    public ITransactionalUnitOfWork CreateTransactional()
    {
        return new TransactionalUnitOfWork(dbContext, userManager, modelMapper);
    }
}