using Abp.Application.Services;
using System;

namespace QBOTest.Web.QuickbookDesktop
{


	public interface IUserAuthentication : IApplicationService
	{
		//   /// <summary>
		//   /// Some implementations will require an authentication context
		//   /// upon creation. Others may require this on login. Some may
		//   /// may requiret both.
		//   /// </summary>
		//   /// <param name="context"></param>
		//   /// <returns></returns>
		//void CreateAuthentication(Object context);

		//   /// <summary>
		//   /// Request authentication and return the implementation appropriate
		//   /// token if successful. Otherwise throw an exception descending from
		//   /// AuthenticationException.
		//   /// </summary>
		//   /// <param name="username"></param>
		//   /// <param name="password"></param>
		//   /// <param name="context"></param>
		//   /// <returns></returns>
		//   Object authenticate(String username, String password, Object context);
		object AuthenticateUser(string username, string password);
	}


	public class AuthenticateException : Exception
	{
		public AuthenticateException(string reason)
		{
		}

		public AuthenticateException()
		{
		}
	}

	/// <summary>
	///     The four "invalid" exceptions are subclasses of this
	///     to make programming for these cases easier.
	/// </summary>
	public class AuthenticateExceptionInvalid : Exception
	{
	}

	/// <summary>
	///     If underlying implementation cannot distinguish between
	///     failures in username/password/credentials, then this should
	///     be used.
	/// </summary>
	public class AuthenticateExceptionInvalidCredentials : AuthenticateExceptionInvalid
	{
	}

	/// <summary>
	///     The indicated user is not valid. Please do not pass this up to the
	///     user, as that facilitates password hacking. This particular information
	///     should be logged and some "invalid credentials" sort of response should
	///     be given to the user.
	/// </summary>
	public class AuthenticateExceptionInvalidUser : AuthenticateExceptionInvalid
	{
	}

	/// <summary>
	///     The indicated password is not valid. Please do not pass this up to the
	///     user, as that facilitates password hacking. This particular information
	///     should be logged and some "invalid credentials" sort of response should
	///     be given to the user.
	/// </summary>
	public class AuthenticateExceptionInvalidPassword : AuthenticateExceptionInvalid
	{
	}

	/// <summary>
	///     The indicated context is not valid. Please do not pass this up to the
	///     user, as that facilitates password hacking. This particular information
	///     should be logged and some "invalid credentials" sort of response should
	///     be given to the user. NOTE: Often the context is not something the user
	///     can modify, so this may indicate a coding or configuration error.
	/// </summary>
	public class AuthenticateExceptionInvalidContext : AuthenticateExceptionInvalid
	{
	}

	/// <summary>
	///     Something went wrong during the authentication, but the problem is not
	///     permanent in nature (e.g. a network error). The user should be informed
	///     that something went wrong, and perhaps they have something to fix (e.g.
	///     network connectivity), but they should try again to authenticate.
	/// </summary>
	public class AuthenticateExceptionTemporary : AuthenticateException
	{
		public AuthenticateExceptionTemporary(string reason)
		{
		}
	}

	/// <summary>
	///     IAuthentication is the interface for authentication classes. There can be
	///     many ways to determine how QBWC authenticates its users. Possibly
	///     implementations could include
	///     QuickBase, ActiveDirectory, flat-file, etc.
	/// </summary>
}
