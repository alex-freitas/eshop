using System;
using System.Collections.Generic;
using TaskScheduler.Entities;

namespace TaskScheduler.Models.SqlCommandJob
{
    public class RegisterViewModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Cron { get; set; }

        public bool? Enabled { get; set; }

        public string Command { get; set; }

        public Guid? DatabaseInfoId { get; set; }

        public List<DatabaseInfo> DatabasesInfo { get; set; }
    }
}
