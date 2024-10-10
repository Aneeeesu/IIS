using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Entities;

namespace IISBackend.DAL;
public class ProjectDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
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
