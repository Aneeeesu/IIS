using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL;
public class ProjectDbContext(DbContextOptions contextOptions) : IdentityDbContext<UserEntity, RoleEntity, Guid>(contextOptions)
{
    public DbSet<AnimalEntity> AnimalEntities => Set<AnimalEntity>();
    public DbSet<ReservationRequestEntity> ReservationRequestEntities => Set<ReservationRequestEntity>();
    public DbSet<VerificationRequestEntity> VerificationRequestEntities => Set<VerificationRequestEntity>();
    public DbSet<HealthRecordEntity> HealthRecordsEntities => Set<HealthRecordEntity>();
    public DbSet<ScheduleEntryEntity> ScheduleEntities => Set<ScheduleEntryEntity>();
    public DbSet<PendingFileUploadEntity> PendingFileUploadEntities => Set<PendingFileUploadEntity>();
    public DbSet<FileEntity> FileEntities => Set<FileEntity>();


    public bool IsSeeded() => !(Users.Count() == 0) && !(Roles.Count() == 0);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.ReservationRequests)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalEntity>()
            .HasMany(x => x.ReservationRequests)
            .WithOne(x => x.Animal)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.ScheduleEntries)
            .WithOne(x => x.User);

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

        modelBuilder.Entity<UserEntity>()
            .HasOne(x => x.Image)
            .WithMany(x=>x.UserImages);

        modelBuilder.Entity<FileEntity>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.Files)
            .HasForeignKey(f => f.OwnerId);

        modelBuilder.Entity<FileEntity>()
            .HasIndex(f => f.Url)
            .IsUnique();

        modelBuilder.Entity<AnimalEntity>()
            .HasOne(x => x.Image)
            .WithMany(x=>x.AnimalImages);

        modelBuilder
            .Entity<PendingFileUploadEntity>()
            .HasOne(x => x.Uploader)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReservationRequestEntity>()
            .HasOne(x => x.TargetSchedule)
            .WithMany(x=>x.WalkRequests)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
