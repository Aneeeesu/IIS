using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IUserFacade
        : IFacade
    {
        Task<List<UserDetailModel>> GetAsync();
        Task<UserDetailModel?> CreateAsync(UserCreateModel model);
        Task<UserDetailModel?> UpdateAsync(UserCreateModel model, ClaimsPrincipal? userPrincipal = null);
    }
}
