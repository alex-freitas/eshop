using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Ordering.Infrastructure.Extensions
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

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });

                    retry.Execute(() => InvokeSeeder(seeder, context, services));
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
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
