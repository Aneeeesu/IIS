using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;

namespace IISBackend.BL.Authorization;

public class UserIsAllowedToApproveRequestAuthorizationHandler : AuthorizationHandler<UserIsAllowedToApproveRequestRequirement, ReservationRequestEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserIsAllowedToApproveRequestRequirement requirement,
        ReservationRequestEntity resource)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Vet"))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Caregiver") && resource.Type == Common.Enums.ScheduleType.walk)
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