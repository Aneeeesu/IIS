﻿using IISBackend.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IISBackend.DAL.Extensions
{
    public static class DbContextExtensions
    {
        public static ProjectDbContext Clone(this DbContext existingContext,DALOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectDbContext>();
            optionsBuilder.UseMySQL(options.ConnectionString);

            // Ensure the connection is not disposed when the original context is disposed
            optionsBuilder.UseInternalServiceProvider(existingContext.GetInfrastructure<IServiceProvider>());

            return new ProjectDbContext(optionsBuilder.Options);
        }
        
    }
}
