using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IISBackend.BL.Models;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IAnimalFacade
        : IFacade<AnimalEntity,AnimalCreateModel, AnimalListModel, AnimalDetailModel>
    {
    }
}
