using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskScheduler.Infrastructure
{
    public static class HostExtensions
    {
        public static IHost MigrateDbContext<T>(this IHost host, Action<T, IServiceProvider> seeder)
            where T : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<T>>();
                var context = services.GetRequiredService<T>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(T).Name);

                    InvokeSeeder(seeder, context, services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(T).Name);
                }
            }

            return host;
        }

        private static void InvokeSeeder<T>(Action<T, IServiceProvider> seeder, T context, IServiceProvider services)
            where T : DbContext
        {
            if (context.Database.IsSqlite())
            {
                var databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            else
            {
                context.Database.Migrate();
            }

            seeder(context, services);
        }
    }
}
