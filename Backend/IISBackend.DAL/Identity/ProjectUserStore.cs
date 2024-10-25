using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace IISBackend.DAL.Authorization;

public class ProjectUserStore : UserStore<UserEntity, IdentityRole<Guid>, ProjectDbContext, Guid>
{
    public ProjectUserStore(ProjectDbContext context)
        : base(context)
    {
        AutoSaveChanges = false; // avoids autosave
    }
}
