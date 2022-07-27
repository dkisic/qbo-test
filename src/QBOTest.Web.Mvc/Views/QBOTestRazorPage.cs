using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace QBOTest.Web.Views
{
    public abstract class QBOTestRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected QBOTestRazorPage()
        {
            LocalizationSourceName = QBOTestConsts.LocalizationSourceName;
        }
    }
}
