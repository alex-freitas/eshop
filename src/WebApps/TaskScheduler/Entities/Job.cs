using System;

namespace TaskScheduler.Entities
{
    public abstract class Job
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Cron { get; set; }

        public bool Enabled { get; set; }

        public string JobType { get; protected set; }

        public abstract string Execute();
    }
}
