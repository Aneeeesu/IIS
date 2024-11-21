using IISBackend.BL.Models.Requests;
using IISBackend.DAL.Entities;
using System.Security.Claims;

namespace IISBackend.BL.Facades.Interfaces;

public interface IVerificationRequestFacade : IFacadeCRUD<VerificationRequestEntity, VerificationRequestCreateModel, VerificationRequestListModel, VerificationRequestDetailModel>
{
    Task<VerificationRequestDetailModel> AuthorizedCreateAsync(VerificationRequestCreateModel createModel, ClaimsPrincipal userPrincipal);
    Task AuthorizedDeleteAsync(Guid id, ClaimsPrincipal userPrincipal);
    Task ResolveRequestAsync(Guid id, bool approved);
}