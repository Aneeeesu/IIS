using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.BL.Facades.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.BL.Facades;

public class RoleFacade(IUnitOfWorkFactory _unitOfWorkFactory, IAuthorizationService _authService, IMapper _modelMapper) : IFacade
{
    public async Task AddRole()
    {
        var userMan = _unitOfWorkFactory.Create().GetUserManager();
    }
}