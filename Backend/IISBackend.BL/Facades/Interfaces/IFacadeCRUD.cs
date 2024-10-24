using IISBackend.BL.Models.Interfaces;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IFacadeCRUD<TEntity, TCreateModel, TListModel, TDetailModel> : IFacade
    where TEntity : class
    where TCreateModel : class, IModel
    where TListModel : class, IModel
    where TDetailModel : class, IModel
{
    Task DeleteAsync(Guid id);
    Task<TDetailModel?> GetAsync(Guid id);
    Task<IEnumerable<TListModel>> GetAsync();
    Task<TDetailModel?> SaveAsync(TCreateModel model, ClaimsPrincipal? userPrincipal=null);
}
