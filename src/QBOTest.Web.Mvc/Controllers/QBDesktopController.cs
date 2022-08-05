using Abp.Runtime.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QBOTest.Authorization.Users;
using QBOTest.Controllers;
using System;
using System.Threading.Tasks;

namespace QBOTest.Web.Controllers
{
	public class QBDesktopController : QBOTestControllerBase
	{

		private readonly UserManager _userManager;
		private string DirectoryPath;
		public QBDesktopController(UserManager userManager, IWebHostEnvironment env)
		{

			_userManager = userManager;
			DirectoryPath = env.WebRootPath;
		}


		public ActionResult Index()
		{
			return View();
		}

		public ActionResult DownloadCompanyFile()
		{
			var getUser = _userManager.GetUserById(AbpSession.UserId.Value);
			var BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

			string path = DirectoryPath + "/QB-QWCFile.qwc";

			if (!System.IO.File.Exists(path))
			{
				var partcode = "<?xml version='1.0'?>" + Environment.NewLine +
					"<QBWCXML>" + Environment.NewLine +
					"<AppName>QBD Integrator</AppName>" + Environment.NewLine +
					"<AppID></AppID>" + Environment.NewLine +
					"<AppURL>" + BaseUrl + "/QBDService.asmx</AppURL>" + Environment.NewLine +
					"<AppDescription> Easy Integration Between App and Quickbooks!</AppDescription>" + Environment.NewLine +
					"<AppSupport>" + BaseUrl + "</AppSupport>" + Environment.NewLine +
					"<UserName>" + getUser.UserName + "</UserName>" + Environment.NewLine +
					"<OwnerID>{" + Guid.NewGuid() + "}</OwnerID >" + Environment.NewLine +//{41c5209d-e7d9-49c2-a1ca-241d0023ab3b}
					"<FileID>{" + Guid.NewGuid() + "}</FileID >" + Environment.NewLine + //{f295b0c3-5dfd-43a8-8958-2901fc198395}
					"<QBType>QBFS</QBType>" + Environment.NewLine +
					"<Notify>false</Notify>" + Environment.NewLine +
					"<Scheduler></Scheduler>" + Environment.NewLine +
					"<IsReadOnly>false</IsReadOnly>" + Environment.NewLine +
					"</QBWCXML>";
				System.IO.File.WriteAllText(path, partcode);

				//For Download
				byte[] fileBytes = System.IO.File.ReadAllBytes(path);
				string fileName = "QB-QWCFile.qwc";
				if (System.IO.File.Exists(path))
				{
					return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
				}

				return View("Index");
			}
			return View("Index");
		}
	}
}

