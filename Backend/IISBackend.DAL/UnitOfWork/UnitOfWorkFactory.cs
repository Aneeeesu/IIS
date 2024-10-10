using Microsoft.EntityFrameworkCore;

namespace IISBackend.DAL.UnitOfWork;

public class UnitOfWorkFactory(ProjectDbContext dbContext) : IUnitOfWorkFactory
{
    public IUnitOfWork Create() => new UnitOfWork(dbContext);
}
