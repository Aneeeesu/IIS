using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IISBackend.DAL.Authorization;

public class ProjectRoleStore : RoleStore<IdentityRole<Guid>, ProjectDbContext, Guid>
{
    public ProjectRoleStore(ProjectDbContext context)
        : base(context)
    {
        AutoSaveChanges = false; // avoids autosave
    }
}
