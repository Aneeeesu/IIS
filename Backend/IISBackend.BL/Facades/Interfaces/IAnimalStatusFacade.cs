using IISBackend.DAL.Entities;

namespace IISBackend.BL.Facades.Interfaces;

public interface IAnimalStatusFacade : IFacadeCRUD<AnimalStatusEntity, AnimalStatusCreateModel, AnimalStatusListModel, AnimalStatusDetailModel>
{

}