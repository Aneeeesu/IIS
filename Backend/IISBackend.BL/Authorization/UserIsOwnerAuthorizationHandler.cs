using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace IISBackend.API.Authorization;

public class UserIsOwnerAuthorizationHandler : AuthorizationHandler<UserIsOwnerRequirement, UserEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsOwnerRequirement requirement,
        UserEntity resource)
    {
        if (context.User.Identity?.Name == resource.UserName)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}