﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Ordering.Infrastructure.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<T>(this IWebHost webHost, Action<T, IServiceProvider> seeder)
            where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
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

            return webHost;
        }

        private static void InvokeSeeder<T>(Action<T, IServiceProvider> seeder, T context, IServiceProvider services) 
            where T : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
