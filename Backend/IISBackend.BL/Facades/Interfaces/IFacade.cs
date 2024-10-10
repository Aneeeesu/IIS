using IISBackend.BL.Models;

namespace IISBackend.BL.Facades.Interfaces;

public interface IFacade<TEntity, TCreateModel, TListModel, TDetailModel>
    where TEntity : class
    where TCreateModel : class, IModel
    where TListModel : class, IModel
    where TDetailModel : class, IModel
{
    Task DeleteAsync(Guid id);
    Task<TDetailModel?> GetAsync(Guid id);
    Task<IEnumerable<TListModel>> GetAsync();
    Task<TDetailModel?> SaveAsync(TCreateModel model);
}
