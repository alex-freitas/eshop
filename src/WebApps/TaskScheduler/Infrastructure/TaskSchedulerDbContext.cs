using Microsoft.EntityFrameworkCore;
using TaskScheduler.Entities;

namespace TaskScheduler.Infrastructure
{
    public class TaskSchedulerDbContext : DbContext
    {
        public TaskSchedulerDbContext()
        {
        }

        public TaskSchedulerDbContext(DbContextOptions<TaskSchedulerDbContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

        public DbSet<HttpJob> HttpJob { get; set; }

        public DbSet<SqlCommandJob> SqlCommandJob { get; set; }

        public DbSet<WindowsCommandJob> WindowsCommandJob { get; set; }

        public DbSet<DatabaseInfo> DatabaseInfo { get; set; }

        public DbSet<DatabaseType> DatabaseType { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<SqlMailReportJob> SqlMailReportJob { get; set; }
    }
}
