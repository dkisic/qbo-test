using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace QBOTest.Controllers
{
    public abstract class QBOTestControllerBase: AbpController
    {
        protected QBOTestControllerBase()
        {
            LocalizationSourceName = QBOTestConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
