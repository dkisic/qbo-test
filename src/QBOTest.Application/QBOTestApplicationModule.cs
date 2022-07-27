using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using QBOTest.Authorization;

namespace QBOTest
{
    [DependsOn(
        typeof(QBOTestCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class QBOTestApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<QBOTestAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(QBOTestApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
