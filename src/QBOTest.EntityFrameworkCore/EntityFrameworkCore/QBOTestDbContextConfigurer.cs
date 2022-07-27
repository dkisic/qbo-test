using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace QBOTest.EntityFrameworkCore
{
    public static class QBOTestDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<QBOTestDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<QBOTestDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
