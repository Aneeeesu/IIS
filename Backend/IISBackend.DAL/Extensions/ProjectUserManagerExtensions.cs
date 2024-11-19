using IISBackend.DAL.Authorization;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IISBackend.DAL.Extensions
{
    public static class ProjectUserManagerExtensions
    {
        public static UserManager<UserEntity> Clone(this UserManager<UserEntity> existingUserManager, ProjectDbContext newDbContext, IServiceProvider provider)
        {
            // Extract dependencies from the existing UserManager
            var options = existingUserManager.Options;
            var passwordHasher = existingUserManager.PasswordHasher;
            var userValidators = existingUserManager.UserValidators;
            var passwordValidators = existingUserManager.PasswordValidators;
            var keyNormalizer = existingUserManager.KeyNormalizer;
            var errors = existingUserManager.ErrorDescriber;
            var logger = existingUserManager.Logger;

            // Create a new UserStore for the new DbContext
            var userStore = new ProjectUserStore((ProjectDbContext)newDbContext);

            // Create a new UserManager instance using the same dependencies
            return new UserManager<UserEntity>(
                userStore,
                Microsoft.Extensions.Options.Options.Create(options),
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                provider,
                (ILogger<UserManager<UserEntity>>)logger
            );
        }

        public static SignInManager<UserEntity> CreateSignInManager(this UserManager<UserEntity> userManager, IServiceProvider provider)
        {
            var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var claimsFactory = provider.GetRequiredService<IUserClaimsPrincipalFactory<UserEntity>>();
            var options = provider.GetRequiredService<IOptions<IdentityOptions>>();
            var logger = provider.GetRequiredService<ILogger<SignInManager<UserEntity>>>();
            var authenticationSchemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
            var confirmation = provider.GetRequiredService<IUserConfirmation<UserEntity>>();

            return new SignInManager<UserEntity>(
                userManager,
                contextAccessor,
            claimsFactory,
                options,
                logger,
                authenticationSchemeProvider,
                confirmation);
        }

        public static RoleManager<RoleEntity> Clone(this RoleManager<RoleEntity> roleManager, ProjectDbContext newDbContext)
        {
            // Extract dependencies from the existing RoleManager
            var roleStore = new ProjectRoleStore((ProjectDbContext)newDbContext);
            var roleValidators = roleManager.RoleValidators;
            var keyNormalizer = roleManager.KeyNormalizer;
            var errors = roleManager.ErrorDescriber;
            var logger = roleManager.Logger;

            // Create a new RoleManager instance using the same dependencies
            return new RoleManager<RoleEntity>(
                roleStore,
                roleValidators,
                keyNormalizer,
                errors,
                (ILogger<RoleManager<RoleEntity>>)logger
            );
        }
    }
}
