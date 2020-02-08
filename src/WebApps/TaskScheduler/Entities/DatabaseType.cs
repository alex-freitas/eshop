using System;

namespace TaskScheduler.Entities
{
    public class DatabaseType
    {
        public Guid Id { get; set; }

        public string ProviderName { get; set; }
    }
}