using AutoMapper;
using IISBackend.DAL.UnitOfWork;
using IISBackend.DAL.Entities;
using IISBackend.BL.Models;
using IISBackend.BL.Facades.Interfaces;

namespace IISBackend.BL.Facades
{

    public class AnimalFacade(IUnitOfWorkFactory unitOfWorkFactory, IMapper modelMapper) : FacadeBase<AnimalEntity,AnimalCreateModel, AnimalListModel, AnimalDetailModel>(unitOfWorkFactory, modelMapper), IAnimalFacade
    {
        //protected override ICollection<string> IncludesNavigationPathDetail =>
        //    new[] { $"{nameof(ActivityEntity.Subject)}", $"{nameof(ActivityEntity.Scores)}" };
    }
}
