using System.ServiceModel;
using System.Threading.Tasks;

namespace QBOTest.Web.QuickbookDesktop
{
	[ServiceContract(Namespace = "http://developer.intuit.com/")]
	[XmlSerializerFormat]
	public interface IQBDWebService
	{
		[OperationContract]
		int DoAuthenticate(string strUserName, string strPassword);
		[OperationContract]
		string[] authenticate(string strUserName, string strPassword);
		[OperationContract]
		Task<string> sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName,
		string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers);
		[OperationContract]
		Task<int> receiveResponseXML(string ticket, string response, string hresult, string message);
		[OperationContract]
		string closeConnection(string ticket);
		[OperationContract]
		string getLastError(string ticket);
	}
}
