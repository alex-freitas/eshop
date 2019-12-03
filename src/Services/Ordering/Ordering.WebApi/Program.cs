using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Extensions;
namespace Ordering.WebApi
{
    public static class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.MigrateDbContext<OrderingSqliteDbContext>(async (context, services) =>
            {
                var logger = services.GetService<ILogger<OrderingContextSeed>>();

                await new OrderingContextSeed().SeedAsync(context, logger);
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
        }
    }
}
