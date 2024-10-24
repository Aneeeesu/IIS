using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Entities;

public class UserEntity : IdentityUser<Guid>,IEntity
{
}