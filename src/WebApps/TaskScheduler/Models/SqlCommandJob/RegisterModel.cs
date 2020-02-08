using System;

namespace TaskScheduler.Models.SqlCommandJob
{
    public class RegisterModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Cron { get; set; }

        public bool Enabled { get; set; }

        public string Command { get; set; }

        public Guid DatabaseInfoId { get; set; }
    }
}
