using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL;
public class ProjectDbContext(DbContextOptions contextOptions) : IdentityDbContext<UserEntity,IdentityRole<Guid>, Guid>(contextOptions)
{
    public DbSet<AnimalEntity> AnimalEntities => Set<AnimalEntity>();
    public DbSet<ReservationRequestEntity> ReservationRequestEntities => Set<ReservationRequestEntity>();
    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
    public DbSet<HealthRecordsEntity> HealthRecordsEntities => Set<HealthRecordsEntity>();
    public DbSet<ScheduleEntryEntity> ScheduleEntities => Set<ScheduleEntryEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ReservationRequestEntity>().HasOne()
        

    }
}
