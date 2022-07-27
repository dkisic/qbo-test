using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using QBOTest.Authorization.Roles;
using QBOTest.Authorization.Users;
using QBOTest.MultiTenancy;

namespace QBOTest.EntityFrameworkCore
{
    public class QBOTestDbContext : AbpZeroDbContext<Tenant, Role, User, QBOTestDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public QBOTestDbContext(DbContextOptions<QBOTestDbContext> options)
            : base(options)
        {
        }
    }
}
