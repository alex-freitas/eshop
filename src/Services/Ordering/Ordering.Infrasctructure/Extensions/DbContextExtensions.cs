using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static bool IsSqlite(this DbContext dbContext)
        {
            return dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite";
        }
    }
}
