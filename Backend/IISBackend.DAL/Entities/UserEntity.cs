using IISBackend.DAL.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Entities;

public class UserEntity : IdentityUser<Guid>, IEntity
{
    public ICollection<ReservationRequestEntity> ReservationRequests { get; set; }
    public ICollection<ScheduleEntryEntity> ScheduleEntries { get; set; }
    public VerificationRequest? VerificationRequest { get; set; }
}