using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL;
public class ProjectDbContext(DbContextOptions contextOptions) : IdentityDbContext<UserEntity,RoleEntity, Guid>(contextOptions)
{
    public DbSet<AnimalEntity> AnimalEntities => Set<AnimalEntity>();
    public DbSet<ReservationRequestEntity> ReservationRequestEntities => Set<ReservationRequestEntity>();
    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
    public DbSet<HealthRecordEntity> HealthRecordsEntities => Set<HealthRecordEntity>();
    public DbSet<ScheduleEntryEntity> ScheduleEntities => Set<ScheduleEntryEntity>();


    public bool IsSeeded() => !(Users.Count() == 0) && !(Roles.Count() == 0);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.ReservationRequests)
            .WithOne(x => x.Voluteer)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalEntity>()
            .HasMany(x => x.ReservationRequests )
            .WithOne(x => x.Animal)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.ScheduleEntries)
            .WithOne(x => x.Volunteer)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalEntity>()
            .HasMany(x => x.ScheduleEntries)
            .WithOne(x => x.Animal)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalEntity>()
            .HasMany(x => x.HealthRecords)
            .WithOne(x => x.Animal)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserEntity>()
            .HasOne(x => x.VerificationRequest)
            .WithOne(x => x.Requestee)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
