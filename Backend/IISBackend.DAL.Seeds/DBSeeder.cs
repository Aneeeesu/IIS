using IISBackend.DAL.Entities;
using IISBackend.DAL.Options;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Seeds;

public class DBSeeder(ProjectDbContext context, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager, DALOptions dALOptions)
{
    public void Seed(string adminPassword)
    {
        if (context.IsSeeded())
            return;
        ManagementSeeds.Seed(adminPassword, context, userManager, roleManager, dALOptions).Wait();
        if(dALOptions.SeedData)
        {
            DevelopmentSeeds.Seed(context, userManager).Wait();
        }
    }

    public static async Task SeedUserWithRolesAsync(UserManager<UserEntity> userManager, ProjectDbContext context, UserEntity user, string password, string[] roles)
    {
        await userManager.CreateAsync(user, password);
        await context.SaveChangesAsync();
        var userEntity = await userManager.FindByNameAsync(user.UserName!);
        foreach (var role in roles)
        {
            await userManager.AddToRoleAsync(userEntity!, role);
            await context.SaveChangesAsync();
        }
    }
}
