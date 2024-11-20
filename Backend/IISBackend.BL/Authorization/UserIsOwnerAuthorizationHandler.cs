using IISBackend.DAL.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IISBackend.BL.Authorization;

public class UserIsOwnerAuthorizationHandler : AuthorizationHandler<UserIsOwnerRequirement, IUserAuthorized>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsOwnerRequirement requirement,
        IUserAuthorized resource)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            // Compare the claim user ID with the resource's OwnerId
            if (userId == resource.GetOwnerID())  // Assuming OwnerId is a Guid on IUserOwnable
            {
                context.Succeed(requirement);
            }
            else if (context.User.IsInRole("Admin")) // Admins can do anything
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        return Task.CompletedTask;
    }
}
