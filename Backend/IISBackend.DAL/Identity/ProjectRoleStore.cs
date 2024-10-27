using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IISBackend.DAL.Entities;

namespace IISBackend.DAL.Authorization;

public class ProjectRoleStore : RoleStore<RoleEntity, ProjectDbContext, Guid>
{
    public ProjectRoleStore(ProjectDbContext context)
        : base(context)
    {
        AutoSaveChanges = false; // avoids autosave
    }
}
