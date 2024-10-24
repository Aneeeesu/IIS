using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IUserFacade
        : IFacadeCRUD<UserEntity, UserCreateModel, UserListModel, UserDetailModel>
    {
    }
}
