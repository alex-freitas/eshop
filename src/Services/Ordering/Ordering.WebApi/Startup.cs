using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Application.Queries;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Repositories;

namespace Ordering.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderQueries>(x => new OrderSqliteQueries(Configuration["ConnectionString"]));

            services
                .AddCustomSqliteDbContext(Configuration)
                .AddCustomMediatR()
                .AddCustomSwagger(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
