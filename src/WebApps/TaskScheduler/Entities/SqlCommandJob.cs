using System;

namespace TaskScheduler.Entities
{
    public class SqlCommandJob : Job
    {
        public SqlCommandJob()
        {
            JobType = "SqlCommandJob";
        }

        public string Command { get; set; }

        public Guid DatabaseInfoId { get; set; }

        public virtual DatabaseInfo DatabaseInfo { get; set; }

        public override string Execute()
        {
            using (var conn = DatabaseInfo.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;

                conn.Open();

                cmd.CommandTimeout = 30 * 60;

                cmd.ExecuteNonQuery();

                return Command;
            }
        }
    }
}