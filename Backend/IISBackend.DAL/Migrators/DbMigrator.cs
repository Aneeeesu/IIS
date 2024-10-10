using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using IISBackend.DAL.Options;

namespace IISBackend.DAL.Migrators;
internal class DbMigrator(ProjectDbContext dbContext, DALOptions options) : IDbMigrator
{
    public void Migrate()
    {
        if (options.RecreateDatabase)
        {
            dbContext.Database.EnsureDeleted();
        }
        dbContext.Database.Migrate();
    }
}
