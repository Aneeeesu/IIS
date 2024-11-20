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
    };

    public static async Task SeedUserWithRolesAsync(UserManager<UserEntity> userManager, ProjectDbContext context, UserEntity user, string password, string[] roles)
    {
        await userManager.CreateAsync(user, password);
        await context.SaveChangesAsync();
        foreach (var role in roles)
        {
            await userManager.AddToRoleAsync(user, role);
            await context.SaveChangesAsync();
        }
    }

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

        await SeedUserWithRolesAsync(userManager, context, AdminUserEntity, adminPassword, roles);
    }

    //public static readonly AnswerEntity AnswerEntity = new()
    //{
    //    Id = Guid.Parse(input: "3821d743-36b1-4ccc-92e2-7571c0b857a5"),
    //    Correct = false,
    //    ImageURL = "",
    //    Text = "a",
    //    Type = AnswerType.WithText,
    //    Question = QuestionSeeds.QuestionEntity
    //};

    //public static readonly AnswerEntity AnswerEntityBase = AnswerEntity with { Id = Guid.Parse("814c5d83-20d5-4e22-a132-9796aeea239c") };
    //public static readonly AnswerEntity AnswerEntityUpdate = AnswerEntity with { Id = Guid.Parse("1cdb039e-f323-429a-905a-7ead8602e468") };
    //public static readonly AnswerEntity AnswerEntityDelete = AnswerEntity with { Id = Guid.Parse("db397c0d-2c08-4f23-bc6b-86cead2ebb11") };

    //public static void Seed(this ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<UserEntity>().HasData(
    //        AnswerEntityBase,
    //        AnswerEntityUpdate,
    //        AnswerEntityDelete,
    //        AnswerEntity
    //    );
    //}
}
