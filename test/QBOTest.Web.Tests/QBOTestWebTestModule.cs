using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using QBOTest.EntityFrameworkCore;
using QBOTest.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace QBOTest.Web.Tests
{
    [DependsOn(
        typeof(QBOTestWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class QBOTestWebTestModule : AbpModule
    {
        public QBOTestWebTestModule(QBOTestEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(QBOTestWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(QBOTestWebMvcModule).Assembly);
        }
    }
}