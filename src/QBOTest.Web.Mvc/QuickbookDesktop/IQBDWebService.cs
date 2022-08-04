using System.ServiceModel;

namespace QBOTest.Web.QuickbookDesktop
{
	[ServiceContract(Namespace = "http://developer.intuit.com/")]
	[XmlSerializerFormat]
	public interface IQBDWebService
	{
		[OperationContract]
		void DoAuthenticate(string strUserName, string strPassword, ref string[] authReturn);
		[OperationContract]
		string[] authenticate(string strUserName, string strPassword);
		[OperationContract]
		string sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName,
		string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers);
	}
}
