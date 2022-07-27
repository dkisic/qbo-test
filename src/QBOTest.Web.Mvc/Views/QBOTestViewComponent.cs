using Abp.AspNetCore.Mvc.ViewComponents;

namespace QBOTest.Web.Views
{
    public abstract class QBOTestViewComponent : AbpViewComponent
    {
        protected QBOTestViewComponent()
        {
            LocalizationSourceName = QBOTestConsts.LocalizationSourceName;
        }
    }
}
