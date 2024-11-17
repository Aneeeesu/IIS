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
    }
}
