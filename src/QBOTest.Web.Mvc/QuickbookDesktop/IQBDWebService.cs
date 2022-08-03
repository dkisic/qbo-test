using System.ServiceModel;

namespace QBOTest.Web.QuickbookDesktop
{
	[ServiceContract(Namespace = "http://developer.intuit.com/")]
	public interface IQBDWebService
	{
		[OperationContract]
		void DoAuthenticate(string strUserName, string strPassword, ref string[] authReturn);
		[OperationContract]
		string[] authenticate(string strUserName, string strPassword);
	}
}
