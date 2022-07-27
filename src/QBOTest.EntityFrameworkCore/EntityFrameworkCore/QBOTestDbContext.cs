using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using QBOTest.Authorization.Roles;
using QBOTest.Authorization.Users;
using QBOTest.MultiTenancy;
using QBOTest.QuickBooks;
using QBOTest.Partners;
using QBOTest.Items;

namespace QBOTest.EntityFrameworkCore
{
    public class QBOTestDbContext : AbpZeroDbContext<Tenant, Role, User, QBOTestDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<QuickBooksSyncLog> QuickBooksSyncLogs { get; set; }
        public DbSet<QuickBooksToken> QuickBooksTokens { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Partner> Partners { get; set; }

        public QBOTestDbContext(DbContextOptions<QBOTestDbContext> options)
            : base(options)
        {
        }
    }
}
