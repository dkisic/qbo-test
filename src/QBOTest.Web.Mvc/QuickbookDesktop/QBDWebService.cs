using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QBOTest.QuickBooksDesktop;
using QBOTest.Users;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace QBOTest.Web.QuickbookDesktop
{

	public class QBDWebService : IQBDWebService
	{
		//Private Variables
		private readonly IUserAppService _userAppService;
		private readonly IUserAuthentication _authentication;


		//private readonly BaseClass bClass = new BaseClass();
		//private readonly IController controller = new RequestController();
		private readonly ISessionPool sessionPool = new MemorySession();

		private readonly IContainer components = null;



		public QBDWebService(IUserAppService userAppService, IUserAuthentication authentication)
		{
			_userAppService = userAppService;
			_authentication = authentication;

		}





		public void DoAuthenticate(string strUserName, string strPassword, ref string[] authReturn)
		{
			//authenticator.CreateAuthentication(null);
			authReturn[0] = (string)_authentication.AuthenticateUser(strUserName, strPassword);
		}


		
		public string clientVersion(string strVersion)
		{
			
			string retVal = null;
			var recommendedVersion = 2.0;
			var supportedMinVersion = 2.0;
			var suppliedVersion = Convert.ToDouble(ParseForVersion(strVersion));
			if (suppliedVersion < supportedMinVersion)
				retVal = "E:You need to upgrade your QBWebConnector";
			else if (suppliedVersion < recommendedVersion)
				retVal = "W:We recommend that you upgrade your QBWebConnector";
			
			return retVal;
		}


		public string[] authenticate(string strUserName, string strPassword)
		{
			
			var authReturn = InitAuthResponse();
			var sess = new QuickBookSession(authReturn[0], strUserName, strPassword);
			try
			{

				DoAuthenticate(strUserName, strPassword, ref authReturn);
				//if (CheckMatchDataSetting(sess) != true && CheckImportSetting(sess) != true &&
				//	CheckTaxImportSetting(sess) != true)
				//{
				//	CheckWork(ref authReturn, sess);
				//	CheckAccounts(ref authReturn, sess);
				//}

				if (authReturn[0] == null)
				{
					authReturn = SetAuthResponse("", "nvu");
					sess = null;
				}
			}
			catch (AuthenticateExceptionInvalid e)
			{
				//LogEvent(e.Message);
				authReturn = SetAuthResponse("", "nvu");
				sess = null;
				//e.QBLogRequest("QuickBookDesktopExport.authenticate \t  => ");

			}
			catch (Exception e2)
			{
				//LogEvent(e2.Message);
				authReturn = SetAuthResponse("", "none");
				sess = null;
				//e2.QBLogRequest("QuickBookDesktopExport.authenticate \t  => ");
			}

			if (sess != null) sessionPool.Put(authReturn[0], sess);
			
			return authReturn;
		}

		private string ParseForVersion(string input)
		{
			// This method is created just to parse the first two version components
			// out of the standard four component version number:
			// <Major>.<Minor>.<Release>.<Build>
			// 
			// As long as you get the version in right format, you could use
			// any algorithm here. 
			string retVal;
			var version = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)(\.\w+){0,2}$", RegexOptions.Compiled);
			var versionMatch = version.Match(input);
			if (versionMatch.Success)
			{
				var major = versionMatch.Result("${major}");
				var minor = versionMatch.Result("${minor}");
				retVal = major + "." + minor;
			}
			else
			{
				retVal = input;
			}

			return retVal;
		}
		private string[] InitAuthResponse()
		{
			// Code below uses a random GUID to use as session ticket
			// An example of a GUID is {85B41BEE-5CD9-427a-A61B-83964F1EB426}
			return SetAuthResponse(Guid.NewGuid().ToString(), null);
		}
		private string[] SetAuthResponse(string ticket, string cfn)
		{
			//It prepare response for authenticate request
			var authRet = new string[4];

			if (ticket != null) authRet[0] = ticket;
			if (cfn != null) authRet[1] = cfn;
			authRet[3] = "3600"; //will override the user auto update time to 1 hours
			return authRet;
		}
	}
}
