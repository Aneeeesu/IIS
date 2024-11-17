using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IISBackend.BL.Models.Animal;
using IISBackend.BL.Models.Schedules;
using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces
{
    public interface IAnimalFacade
        : IFacadeCRUD<AnimalEntity,AnimalCreateModel, AnimalListModel, AnimalDetailModel>
    {
    }
}
