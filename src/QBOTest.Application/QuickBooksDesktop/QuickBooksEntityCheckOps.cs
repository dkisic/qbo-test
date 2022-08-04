using Abp.Domain.Repositories;
using QBOTest.Partners;
using QBOTest.QuickBooksDesktop.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static QBOTest.QuickBooksDesktop.RequestController;
//using static QBOTest.QuickBooksDesktop.RequestController;

namespace QBOTest.QuickBooksDesktop
{

	public class QuickBooksEntityCheckOps : BaseClass
	{
		private readonly string customersIds = "CustomersIds";
		private readonly IRepository<Partner, Guid> _partnerRepository;

		public QuickBooksEntityCheckOps(IRepository<Partner, Guid> partnerRepository) { 
		_partnerRepository = partnerRepository;
		}

		public string QueryCustomers(QuickBookSession sess)
		{
			
			try
			{
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) == null) return EmptyRequest();

				var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
				var dbList = (List<Guid>)lists;

				var customers = dbList.Take(SendDataRecordSize).ToList();
				var updatedList = dbList.Except(customers).ToList();
				sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), updatedList);
				var customerModRqXML = BuildRqXML(sess, customers, true);
				return customerModRqXML;
			}
			catch (Exception exp)
			{
				sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), null);

				//return empty request so flow continue
				return EmptyRequest();
			}
		}

		private string BuildRqXML(QuickBookSession sess, List<Guid> customers, bool isQuery = false)
		{
			var requestSet = new XmlDocument();
			requestSet.AppendChild(requestSet.CreateXmlDeclaration("1.0", "utf-8", null));
			requestSet.AppendChild(requestSet.CreateProcessingInstruction("qbxml", "version=\"13.0\""));

			var qbXML = requestSet.CreateElement("QBXML");
			requestSet.AppendChild(qbXML);

			var qbXMLMsgsRq = requestSet.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "continueOnError");


			var getAllCustomers = _partnerRepository.GetAllList(x => customers.Contains(x.Id)).ToList();


			//for querying Request
			if (isQuery)
			{
				BuildCustomerQueryRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);
				return requestSet.OuterXml;
			}

			return requestSet.OuterXml;
		}

		private void BuildCustomerQueryRequest(List<Partner> customers, QuickBookSession sess, XmlElement qbXMLMsgsRq,
			ref XmlDocument requestSet)
		{
			var listIds = new List<UtilityClass>();
			var counter = 0;
			foreach (var cust in customers)
				try
				{
					var custQueryReq = requestSet.CreateElement("CustomerQueryRq");
					qbXMLMsgsRq.AppendChild(custQueryReq);
					custQueryReq.SetAttribute("requestID", counter.ToString());

					var idsClass = new UtilityClass
					{
						RequestId = counter.ToString(),
						EntityID = cust.Id
					};
					listIds.Add(idsClass);

					custQueryReq.AppendChild(requestSet.CreateElement("ListID")).InnerText = cust.QuickBooksDesktopId?.Trim();
					counter++;
				}
				catch (Exception exp)
				{

					//remove the current Request from Xml tree as exception occured and may cause error in QB
					var qbXMLMsgsRsNodeList = requestSet.GetElementsByTagName("QBXMLMsgsRq");
					var childNode =
						qbXMLMsgsRsNodeList.Cast<XmlNode>().Select(x => x.LastChild).FirstOrDefault();
					if (childNode != null)
						qbXMLMsgsRq.RemoveChild(childNode);

					listIds.RemoveAll(x => x.RequestId == counter.ToString());
				}

			sess.SetProperty(customersIds, listIds);
		}

		public async Task  GetCustomersFromQuery(string response, QuickBookSession sess)
		{ 
			try
			{
				var outputXMLDoc = new XmlDocument();
				
				outputXMLDoc.LoadXml(response);
				var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerQueryRs");

				var customersIdsSess = sess.GetProperty(customersIds);
				var customersIdsList = (List<UtilityClass>)customersIdsSess;
				var count = qbXMLMsgsRsNodeList.Count;
				for (var x = 0; x < count; x++)
					try
					{
						var rsAttributes = qbXMLMsgsRsNodeList.Item(x).Attributes;
						var retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
						var retRequestID = rsAttributes.GetNamedItem("requestID").Value;

						//get the response node for detailed info  and a response contains max one childNode for "CustomerRet"
						var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(x).ChildNodes;
						if (qbXMLMsgsRsNodeList.Item(x).Name == "CustomerQueryRs" && retStatusCode == "500")
						{
							var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);

							//Remove qb mapping when adding customer it will Add these customers
							var customer =  _partnerRepository.FirstOrDefault(x => x.Id == objIds.EntityID);
							if (customer != null)
							{
								customer.QuickBooksDesktopId = null;
								await _partnerRepository.UpdateAsync(customer);


							}
						}
						else if (custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0).Name.Equals("CustomerRet") &&
								 (retStatusCode == "0" || retStatusCode == "530"))
						{
							var custRetNodeList = custAddRsNodeList.Item(0).ChildNodes;
							//var qbDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID")
							//	.Select(z => z.InnerText).FirstOrDefault();
							//var editSequence = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "EditSequence")
							//	.Select(z => z.InnerText).FirstOrDefault();
							//var name = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "FullName")
							//	.Select(z => z.InnerText).FirstOrDefault();
							var isActive = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "IsActive")
								.Select(z => z.InnerText).FirstOrDefault();
							if (isActive == "false")
							{
								//var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);

								////Set Synced False so it will in customer updates set customer status to Active in Qb
								//var customer = _partnerRepository.FirstOrDefault(x => x.Id == objIds.EntityID);
								//customer.IsQBSynced = false;
								//customer.IsQBActive = true;
								//_customerService.UpdateCustomerFirst(customer);


							}
						}

						var i = GetCount(sess);
					

					}
					catch (Exception exp)
					{

					}

				//remove previous Ids and add new List and Edit sequence against those entity which are going to update
				sess.RemoveProperty(customersIds);
				//SetQueryListIds(sess, listIds);
			}
			catch (Exception exp)
			{

			}
		}


	}
}
