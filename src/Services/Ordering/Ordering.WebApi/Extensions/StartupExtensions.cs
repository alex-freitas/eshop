using System;
using System.IO;
using System.Reflection;
using FluentValidation;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ordering.Application.Behaviors;
using Ordering.Application.Commands;
using Ordering.Application.IntegrationsEvents;
using Ordering.Application.Queries;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Repositories;

namespace Ordering.WebApi.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
        {
            var assembly = typeof(CreateOrderCommand).Assembly;

            // duvida
            var assemblyScanner = AssemblyScanner.FindValidatorsInAssembly(assembly);
            //assemblyScanner.ForEach(r => services.AddScoped(r.InterfaceType, r.ValidatorType));

            foreach (var result in assemblyScanner)
            {
                services.AddScoped(result.InterfaceType, result.ValidatorType);
            }

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBahavior<,>));

            services.AddMediatR(assembly);

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IBuyerRepository, BuyerRepository>();
            services.AddTransient<IOrderQueries>(x => new OrderSqliteQueries(connectionString));

            //var migrationsAssembly = typeof(OrderingContext).GetTypeInfo().Assembly.GetName().Name; 
            var migrationsAssembly = "Ordering.Infrastructure";

            services.AddEntityFrameworkSqlServer();

            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
            ServiceLifetime.Scoped);

            return services;
        }

        public static IServiceCollection AddCustomInMemoryDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseInMemoryDatabase("EShop.Ordering");
            });

            return services;
        }

        public static IServiceCollection AddCustomSqliteDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];

            services.AddScoped<IOrderRepository, OrderSqliteRepository>();
            services.AddScoped<IBuyerRepository, BuyerSqliteRepository>();

            services.AddTransient<IOrderQueries>(x => new OrderSqliteQueries(connectionString));

            services.AddDbContext<OrderingContext>(SqliteContextConfiguration(connectionString));

            services.AddDbContext<IntegrationEventLogContext>(SqliteContextConfiguration(connectionString));

            return services;
        }
        
        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();
            
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            return services;
        }

        #region Swagger
        public static IServiceCollection AddCustomSwashbuckleSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ordering API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Alex Freitas",
                        Email = string.Empty,
                        Url = new Uri("https://www.linkedin.com/in/alexabreufreitas/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.   
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwashbuckleSwagger(this IApplicationBuilder app)
        {
            //app.UseSwagger();
            SwaggerBuilderExtensions.UseSwagger(app);

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API V1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }

        public static IServiceCollection AddCustomNSwagSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerDocument();
            return services;
        }

        public static IApplicationBuilder UseCustomNSwagSwagger(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            
            app.UseSwaggerUi3(settings =>
            {
                settings.EnableTryItOut = true;
            });

            return app;
        }
        #endregion

        private static Action<DbContextOptionsBuilder> SqliteContextConfiguration(string connectionString)
        {
            return options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("Ordering.Infrastructure"));
            };
        }
    }
}
