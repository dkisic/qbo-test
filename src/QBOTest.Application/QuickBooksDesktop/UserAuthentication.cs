﻿using QBOTest.Users;
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
		private string userTicket;
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

			return userTicket;
		}

		/// <summary>
		///     Get a new userTicket for the indicated user and password.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns>string</returns>
		public async Task<string> GetTicketAsync(string username, string password)
		{
			if (userTicket != null) return userTicket;
			if (!(await _userAppService.ValidateUser(username, password)))
			{
				return null;
			}
			else
			{
				userTicket = Guid.NewGuid().ToString();
			}

			return userTicket;
		}



	}
}
