using System;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ordering.Application.Behaviors;
using Ordering.Application.Commands;
using Ordering.Infrasctructure;

namespace Ordering.WebApi
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Version = "v1",
                    Title = "Ordering API",                     
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //var migrationsAssembly = typeof(OrderingContext).GetTypeInfo().Assembly.GetName().Name; 
            var migrationsAssembly = "Ordering.Infrasctructure";

            services.AddEntityFrameworkSqlServer();

            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseSqlServer(
                    configuration["ConnectionString"],
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
    }
}
