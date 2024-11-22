using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;

namespace IISBackend.BL.Authorization;

public class UserAllowedToManageScheduleAuthorizationHandler : AuthorizationHandler<UserAllowedToManageScheduleRequirement, ScheduleEntryEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserAllowedToManageScheduleRequirement requirement,
        ScheduleEntryEntity resource)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Vet"))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Caregiver") && (resource.Type == Common.Enums.ScheduleType.walk || resource.Type == Common.Enums.ScheduleType.availableForWalk))
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
