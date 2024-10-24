using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL;
public class ProjectDbContext(DbContextOptions contextOptions) : IdentityDbContext<UserEntity,IdentityRole<Guid>, Guid>(contextOptions)
{
    public DbSet<AnimalEntity> AnimalEntities => Set<AnimalEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<SubjectEntity>()
        //.HasMany<ActivityEntity>(s => s.Activities)
        //.WithOne(a => a.Subject)
        //.OnDelete(DeleteBehavior.Cascade);

    }
}
