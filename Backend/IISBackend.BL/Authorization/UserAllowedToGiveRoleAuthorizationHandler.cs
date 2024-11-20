using IISBackend.API.Authorization;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;

namespace IISBackend.BL.Authorization;

public class UserAllowedToGiveRoleAuthorizationHandler : AuthorizationHandler<UserAllowedToGiveRoleRequirement, UserUpdateModel>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserAllowedToGiveRoleRequirement requirement,
        UserUpdateModel resource)
    {
        if (resource.Roles == null)
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Vet") && !resource.Roles.Any(o => o == "Admin" || o == "Vet")) // Admins can do anything
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }
}