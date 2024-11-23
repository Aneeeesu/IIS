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
        FirstName = "Test",
        LastName = "Vet",
    };
    public static readonly UserEntity CaregiverUserEntity = new()
    {
        Id = Guid.NewGuid(),
        UserName = "TestCaregiver",
        FirstName = "Test",
        LastName = "Caregiver",
    };


    public static async Task Seed(ProjectDbContext context, UserManager<UserEntity> userManager)
    {
        await DBSeeder.SeedUserWithRolesAsync(userManager, context, VetUserEntity, "Heslo_123", ["Vet"]);
        await DBSeeder.SeedUserWithRolesAsync(userManager, context, CaregiverUserEntity, "Heslo_123", ["Caregiver"]);
    }
}
