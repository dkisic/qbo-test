using QBOTest.Users;
using QBOTest.Web.QuickbookDesktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	public class UserAuthentication : IUserAuthentication
	{
		private int TenantId;
		private readonly IUserAppService _userAppService;
		//Constructor
		public UserAuthentication(IUserAppService userAppService)
		{
			_userAppService = userAppService;
		}


		public object AuthenticateUser(string username, string password)
		{

			try
			{
				 GetTicketAsync(username, password).ToString();
			}
			catch (Exception e2)
			{
				// A temporary exception, retry later
				new AuthenticateException(e2.ToString());
				
			}
			return TenantId;
		}

		///// <summary>
		/////     Get a new userTicket for the indicated user and password.
		///// </summary>
		///// <param name="username"></param>
		///// <param name="password"></param>
		///// <returns>string</returns>
		public async Task<int> GetTicketAsync(string username, string password)
		{

			TenantId = await _userAppService.ValidateUser(username, password);

			return TenantId;

		}



	}
}
