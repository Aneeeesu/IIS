using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.UnitOfWork;

public class UnitOfWorkFactory(ProjectDbContext dbContext,UserManager<UserEntity> userManager) : IUnitOfWorkFactory
{
    public IUnitOfWork Create() => new UnitOfWork(dbContext, userManager);
}
