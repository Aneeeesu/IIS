namespace IISBackend.DAL.UnitOfWork;

public interface ITransactionalUnitOfWork : IUnitOfWork
{
    Task SaveChangesAsync();
    Task RevertChangesAsync();
}