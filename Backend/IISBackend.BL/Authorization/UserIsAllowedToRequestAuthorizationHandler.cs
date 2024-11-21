using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;

namespace IISBackend.BL.Authorization;

public class UserIsAllowedToRequestAuthorizationHandler : AuthorizationHandler<UserIsAllowedToRequestRequirement, ReservationRequestEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsAllowedToRequestRequirement requirement,
        ReservationRequestEntity resource)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Vet") || context.User.IsInRole("Caregiver"))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Verified volunteer") && resource.Type == Common.Enums.ScheduleType.walk)
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
