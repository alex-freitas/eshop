using System;
using System.Data;
using System.Data.Common;

namespace TaskScheduler.Entities
{
    public class DatabaseInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ConnectionString { get; set; }

        public Guid DatabaseTypeId { get; set; }

        public virtual DatabaseType DatabaseType { get; set; }
        
        public IDbConnection CreateConnection()
        {
            var factory = DbProviderFactories.GetFactory(DatabaseType.ProviderName);

            var connection = factory.CreateConnection();

            if (connection == null)
                throw new Exception($"The Provider '{DatabaseType.ProviderName}' does not exist");

            connection.ConnectionString = ConnectionString;

            return connection;
        }
    }
}