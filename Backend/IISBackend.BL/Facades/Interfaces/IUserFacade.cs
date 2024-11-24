using System.Security.Claims;
using IISBackend.BL.Models;
using IISBackend.BL.Models.User;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IUserFacade
        : IFacade
    {
        Task<SignInResult> Login(LoginModel login);
        Task Logout();
        Guid? GetCurrentUserGuid(ClaimsPrincipal user);
        Task<List<UserListModel>> GetAsync();
        Task<UserDetailModel?> GetUserByIdAsync(Guid id);
        Task<UserDetailModel?> CreateAsync(UserCreateModel model,string? roleName = null);
        Task<UserDetailModel?> UpdateAsync(UserUpdateModel model, ClaimsPrincipal userPrincipal);
        Task<UserDetailModel?> DeleteAsync(Guid id, ClaimsPrincipal userPrincipal);
        Task ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userPrincipal);
    }
}
