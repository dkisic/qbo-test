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
		public string DuplicateRecordTryList = "DuplicateRecordTryList";
		public string QueryingListIDs = "QueryingListIDs";
		public string MaxReturnedRecordSize = "100";
		public string RecordSizeForMatchData = "10";

		public int SendDataRecordSize = 100;


	

		public void PutDuplicateRecordInfo(QuickBookSession sess, Guid id)
		{
			try
			{
				var duplicateSess = sess.GetProperty(DuplicateRecordTryList);
				var duplicateIdsList =
					duplicateSess != null ? (List<UtilityClass>)duplicateSess : new List<UtilityClass>();

				var idsClass = new UtilityClass { EntityID = id };
				duplicateIdsList.Add(idsClass);

				sess.SetProperty(DuplicateRecordTryList, duplicateIdsList);
			}
			catch (Exception ex)
			{

			}
		}

		public bool IsDuplicateRecord1StTry(QuickBookSession sess, Guid id)
		{
			try
			{
				var duplicateSess = sess.GetProperty(DuplicateRecordTryList);
				var duplicateIdsList =
					duplicateSess != null ? (List<UtilityClass>)duplicateSess : new List<UtilityClass>();

				var objIds = duplicateIdsList.FirstOrDefault(y => y.EntityID == id);
				return objIds == null;
			}
			catch (Exception ex)
			{

				return false;
			}
		}

		public void ClearDuplicateRecordInfo(QuickBookSession sess)
		{
			if (sess.GetProperty(DuplicateRecordTryList) != null)
				sess.RemoveProperty(DuplicateRecordTryList);
		}

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

		//public Customer AddRandomNameWihCust(Customer cust)
		//{
		//	var randomName = Randomizer.RandomNumberOfLetters(5);
		//	var custName = "";
		//	var firstName = "";
		//	var lastName = "";

		//	if (string.IsNullOrEmpty(cust.FullName) || string.IsNullOrWhiteSpace(cust.FullName))
		//	{
		//		if (cust.CompanyName != null) custName = cust.CompanyName.Trim();
		//		if (string.IsNullOrEmpty(cust.FirstName))
		//		{
		//			if (cust.Title1 != null) firstName = cust.Title1.Trim();
		//			if (cust.Title1LastName != null)
		//				lastName = cust.Title1LastName.Trim();
		//		}
		//		else
		//		{
		//			firstName = cust.FirstName.Trim();
		//			if (cust.LastName != null) lastName = cust.LastName.Trim();
		//		}

		//		if (string.IsNullOrEmpty(custName.Trim())) custName = $"{(firstName)} {(lastName)}";
		//	}
		//	else
		//	{
		//		custName = cust.FullName.Trim();
		//		firstName = cust.FirstName.Trim();
		//		lastName = cust.LastName.Trim();
		//	}

		//	if (string.IsNullOrEmpty(custName.Trim()))
		//	{
		//		var customerAddress = cust.CustomerAddresses.FirstOrDefault();
		//		if (customerAddress?.CompanyFirstName1 != null)
		//		{
		//			lastName = string.IsNullOrEmpty(customerAddress.CompanyLastName1)
		//				? ""
		//				: customerAddress.CompanyLastName1;
		//			custName = customerAddress.CompanyFirstName1 + " " + lastName;
		//			firstName = customerAddress.CompanyFirstName1;
		//		}
		//	}

		//	//we get the name as we getting it in Request so put these name in session  
		//	cust.FullName = custName.ToCleanStringSub0(35) + " " + randomName;
		//	cust.FirstName = firstName;
		//	cust.LastName = lastName;
		//	return cust;
		//}



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


		

		



		public void RemoveQueryListIds(QuickBookSession sess)
		{
			sess.RemoveProperty(QueryingListIDs);
		}


	}
}
