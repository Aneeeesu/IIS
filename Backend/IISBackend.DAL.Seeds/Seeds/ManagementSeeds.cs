using IISBackend.DAL.Entities;
using IISBackend.DAL.Options;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Seeds;

public static class ManagementSeeds
{
    public static readonly UserEntity AdminUserEntity = new()
    {
        Id = Guid.Parse("ccc0e0bf-160a-4730-a2c1-2fb3ef5e02d2"),
        UserName = "Admin",
        FirstName = "Admin",
        LastName = "Admin",
    };

    public static async Task Seed(string adminPassword,ProjectDbContext context, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager, DALOptions dALOptions)
    {
        var roles = new string[] { "Admin", "Vet", "Caregiver", "Volunteer","Verified volunteer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new RoleEntity { Name = role });
            }
        }

        await DBSeeder.SeedUserWithRolesAsync(userManager, context, AdminUserEntity, adminPassword, roles);
    }
}
