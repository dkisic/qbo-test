using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QBOTest.QuickBooksDesktop
{
	public class QuickBookSession
	{
		private readonly string password;
		private readonly string ticket;
		private readonly string username;
		private string companyFileName = "";
		private string companyName = "";
		private string country = "US";
		private short majorVers = 2;
		private short minorVers;
		private string quickBookVersion = "";
		private readonly Hashtable sessionProperties = new Hashtable();

		public QuickBookSession(string myTicket, string username, string password)
		{
			ticket = myTicket;
			this.username = username;
			this.password = password;
		}

		public override string ToString()
		{
			return $"Company:{companyName}, Version:{quickBookVersion}";
		}

		public QuickBookSession()
		{
			ticket = "";
			username = "";
			password = "";
		}

		public void DefineSession(string strCompanyFileName, string strHcpResponse, string qbXMLCountry,
			short qbXMLMajorVers, short qbXMLMinorVers)
		{
			SetNamesAndVersion(strHcpResponse);
			companyFileName = strCompanyFileName;
			country = qbXMLCountry;
			majorVers = qbXMLMajorVers;
			minorVers = qbXMLMinorVers;
		}

		public string GetTicket()
		{
			return ticket;
		}

		public string GetUsername()
		{
			return username;
		}

		public string GetPassword()
		{
			return password;
		}

		public string GetCompanyName()
		{
			return companyName;
		}

		public string GetCompanyFileName()
		{
			return companyFileName;
		}

		public string GetQuickBookVersion()
		{
			return quickBookVersion;
		}

		public string GetCountry()
		{
			return country;
		}

		public short GetMajorVers()
		{
			return majorVers;
		}

		public short GetMinorVers()
		{
			return minorVers;
		}

		public void SetProperty(string name, object value)
		{
			//value.QBLogRequest($"QuickBookSession.SetProperty \t  => Name: {name}  Value:");

			if (sessionProperties.Contains(name))
				sessionProperties[name] = value;
			else
				sessionProperties.Add(name, value);
		}

		public object GetProperty(string name)
		{
			//QBLogHelper.QBLogRequest($"QuickBookSession.GetProperty \t  => Name: {name}");
			return sessionProperties[name];
		}

		public void RemoveProperty(string name)
		{
			sessionProperties.Remove(name);
		}

		public bool IsPropertyExist(string name)
		{
			return sessionProperties.ContainsKey(name);
		}

		public bool IsValueExistInProperty(object value)
		{
			return sessionProperties.ContainsValue(value);
		}

		private void SetNamesAndVersion(string response)
		{
			if (!string.IsNullOrEmpty(response))
			{
				var outputXMLDoc = new XmlDocument();
				outputXMLDoc.LoadXml(response);
				var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("HostRet");
				if (qbXMLMsgsRsNodeList.Count > 0)
				{
					try
					{
						quickBookVersion = qbXMLMsgsRsNodeList[0].Cast<XmlNode>().Where(y => y.Name == "ProductName")
							.Select(z => z.InnerText).FirstOrDefault();
					}
					catch (Exception)
					{
					}

					quickBookVersion = qbXMLMsgsRsNodeList[0].Cast<XmlNode>().Where(y => y.Name == "ProductName")
						.Select(z => z.InnerText).FirstOrDefault();
				}

				var qbXMLMsgsRsNodeList1 = outputXMLDoc.GetElementsByTagName("CompanyQueryRs");

				if (qbXMLMsgsRsNodeList1.Count > 0)
					try
					{
						//var compAddRsNodeList = qbXMLMsgsRsNodeList.Item(0)?.ChildNodes;
						//var custRetNodeList = compAddRsNodeList?.Item(0)?.ChildNodes;
						companyName = qbXMLMsgsRsNodeList1[0].ChildNodes[0].Cast<XmlNode>()
							.Where(y => y.Name == "CompanyName").Select(z => z.InnerText).FirstOrDefault();
					}
					catch (Exception)
					{
					}
			}
		}
	}
}
