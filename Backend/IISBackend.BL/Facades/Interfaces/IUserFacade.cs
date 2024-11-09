using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IUserFacade
        : IFacade
    {
        Task<List<UserListModel>> GetAsync();
        Task<UserDetailModel?> GetUserByIdAsync(Guid id);
        Task<UserDetailModel?> CreateAsync(UserCreateModel model,string? roleName = null);
        Task<UserDetailModel?> UpdateAsync(UserUpdateModel model, ClaimsPrincipal? userPrincipal = null);
        Task<UserDetailModel?> DeleteAsync(Guid id, ClaimsPrincipal? userPrincipal = null);
    }
}
