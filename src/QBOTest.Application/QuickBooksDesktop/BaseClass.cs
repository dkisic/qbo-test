using Microsoft.AspNetCore.Http;
using QBOTest.QuickBooksDesktop.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using System.Xml;

namespace QBOTest.QuickBooksDesktop
{


	public class BaseClass
	{
		public const int ProgressSteps = 10;
		public string CountList = "CountList";
		public string QueryingListIDs = "QueryingListIDs";
		public string MaxReturnedRecordSize = "100";
		public string RecordSizeForMatchData = "10";

		public int SendDataRecordSize = 100;



		public long GetCount(QuickBookSession sess)
		{
			long value;
			if (sess.GetProperty(CountList) != null)
			{
				var countString = sess.GetProperty(CountList);
				var count = (long?)countString ?? 0;
				value = count + 1;
				sess.SetProperty(CountList, value);
			}
			else
			{
				value = 0;
				sess.SetProperty(CountList, value);
			}

			return value;
		}

		public void ClearCount(QuickBookSession sess)
		{
			if (sess.GetProperty(CountList) != null)
				sess.RemoveProperty(CountList);
		}

		



		public string EmptyRequest()
		{
			var requestSet = new XmlDocument();
			requestSet.AppendChild(requestSet.CreateXmlDeclaration("1.0", null, null));
			requestSet.AppendChild(requestSet.CreateProcessingInstruction("qbxml", "version=\"13.0\""));
			var qbXML = requestSet.CreateElement("QBXML");
			requestSet.AppendChild(qbXML);
			var qbXMLMsgsRq = requestSet.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "continueOnError");

			return requestSet.OuterXml;
		}




		public void SetQueryListIds(QuickBookSession sess, List<UtilityClass> list)
		{
			sess.SetProperty(QueryingListIDs, list);
		}

		public List<UtilityClass> GetQueryListIds(QuickBookSession sess)
		{
			var queryingLists = sess.GetProperty(QueryingListIDs);
			var list = queryingLists != null ? (List<UtilityClass>)queryingLists : new List<UtilityClass>();
			return list;
		}


		public void RemoveQueryListIds(QuickBookSession sess)
		{
			sess.RemoveProperty(QueryingListIDs);
		}






	}
}
