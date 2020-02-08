using System.Linq;

namespace TaskScheduler.Infrastructure
{
    public static class JobHelper
    {
        public static string Execute(TaskSchedulerDbContext db, string name)
        {
            var job = db.Jobs.FirstOrDefault(x => x.Name == name);
            return job?.Execute();
        }
    }
}
