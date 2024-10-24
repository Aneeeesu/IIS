using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace IISBackend.DAL.Authorization
{
    public class CustomUserStore : UserStore<UserEntity, IdentityRole<Guid>, ProjectDbContext, Guid>
    {
        public CustomUserStore(ProjectDbContext context)
            : base(context)
        {
            AutoSaveChanges = false; // avoids autosave
        }
    }

}
