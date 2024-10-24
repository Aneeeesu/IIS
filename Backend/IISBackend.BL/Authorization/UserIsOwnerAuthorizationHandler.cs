using IISBackend.BL.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace IISBackend.API.Authorization;

public class UserIsOwnerAuthorizationHandler : AuthorizationHandler<UserIsOwnerRequirement, UserBaseModel>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsOwnerRequirement requirement,
        UserBaseModel resource)
    {
        if (context.User.Identity?.Name == resource.UserName)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}