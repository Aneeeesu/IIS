using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace IISBackend.API.Authorization;

public class UserIsAccountOwnerAuthorizationHandler : AuthorizationHandler<UserIsAccountOwnerRequirement, UserEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsAccountOwnerRequirement requirement,
        UserEntity resource)
    {
        if (context.User.Identity?.Name == resource.UserName)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}