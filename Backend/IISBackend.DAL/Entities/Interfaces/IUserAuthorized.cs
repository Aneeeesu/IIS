using ITUBackend.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITUBackend.API.Entities.Interfaces;
public interface IUserAuthorized
{
    public Guid GetOwnerID();
}
