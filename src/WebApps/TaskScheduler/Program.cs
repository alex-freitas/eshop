using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TaskScheduler.Infrastructure;

namespace TaskScheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.MigrateDbContext<TaskSchedulerDbContext>((context, services) => { });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
