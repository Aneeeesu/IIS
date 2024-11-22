using IISBackend.DAL.Entities;
using IISBackend.DAL.Options;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Seeds;

public static class DevelopmentSeeds
{
    public static readonly UserEntity VetUserEntity = new()
    {
        Id = Guid.NewGuid(),
        UserName = "TestVet",
    };
    public static readonly UserEntity CaregiverUserEntity = new()
    {
        Id = Guid.NewGuid(),
        UserName = "TestCaregiver",
    };


    public static async Task Seed(ProjectDbContext context, UserManager<UserEntity> userManager)
    {
        await DBSeeder.SeedUserWithRolesAsync(userManager, context, VetUserEntity, "Heslo_123", ["Vet"]);
        await DBSeeder.SeedUserWithRolesAsync(userManager, context, CaregiverUserEntity, "Heslo_123", ["Caregiver"]);
    }
}
