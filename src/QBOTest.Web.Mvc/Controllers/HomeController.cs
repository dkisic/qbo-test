using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using QBOTest.Controllers;

namespace QBOTest.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : QBOTestControllerBase
    {
        public ActionResult Index()
        {
            return RedirectToAction("Home", "QuickBooks");
        }
    }
}
