using Abp.Application.Services;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using QBOTest.Partners;
using QBOTest.QuickBooksDesktop.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static QBOTest.QuickBooksDesktop.RequestController;

namespace QBOTest.QuickBooksDesktop
{

	public class QuickBooksCustomerOps : BaseClass, ITransientDependency, IApplicationService
	{
		private const string CustomersIds = "CustomersIds";
		private readonly IRepository<Partner, Guid> _partnerRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		public QuickBooksCustomerOps(IRepository<Partner, Guid> partnerRepository, IUnitOfWorkManager unitOfWorkManager)
		{
			_partnerRepository = partnerRepository;
			_unitOfWorkManager = unitOfWorkManager;
		}


		//Constructor

		#region Export Section

		#region Request

		//public string AddCustomers(QuickBookSession sess)
		//{

		//	try
		//	{

		//		List<Guid> customers;

		//		if (sess.GetProperty(AddRequestDbList.CustomersLists.ToString()) == null)
		//		{
		//			List<Guid> customersids;

		//			//
		//			// Get new customers to sync again and set property

		//			customers = customersids.Take(SendDataRecordSize).ToList();
		//			customersids = customersids.Except(customers).ToList();

		//			sess.SetProperty(AddRequestDbList.CustomersLists.ToString(), customersids);
		//		}
		//		else
		//		{
		//			var lists = sess.GetProperty(AddRequestDbList.CustomersLists.ToString());
		//			var dbList = (List<int>)lists;
		//			customers = dbList.Take(SendDataRecordSize).ToList();

		//			var updatedList = dbList.Except(customers).ToList();
		//			sess.SetProperty(AddRequestDbList.CustomersLists.ToString(), updatedList);
		//		}

		//		var customerAddRqXML = BuildRqXML(sess, customers);
		//		return customerAddRqXML;
		//	}
		//	catch (Exception exp)
		//	{
		//		//return empty request so flow continue
		//		return EmptyRequest();
		//	}
		//}

		public async Task<string> QueryCustomers(QuickBookSession sess)
		{


			try
			{
				List<Guid> customers;
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) == null)
				{
					List<Guid> customersids;

					//GetAllCustomersForQBDesktopUpdate
					customersids = _partnerRepository.GetAllList().Where(x => x.QuickBooksDesktopId != null && x.IsQBSynced == false).Select(x => x.Id).ToList();


					sess.SetProperty(ModRequestDbList.CustomersModLists.ToString(),
						customersids); //adding it to session to get again it from it to mod in Qb
					customers = customersids.Take(100).Distinct().ToList();
					customersids = customersids.Except(customers).ToList();
					sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), customersids);
				}
				else
				{
					var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
					var dbList = (List<Guid>)lists;
					customers = dbList.Take(100).Distinct().ToList();

					var updatedList = dbList.Except(customers).ToList();
					sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), updatedList);
				}

				var customerModRqXML = (string)await BuildRqXML(sess, customers, true);
				return customerModRqXML;
			}
			catch (Exception exp)
			{

				//return empty request so flow continue
				return EmptyRequest();
			}
		}

		public async Task<string> ModCustomersAsync(QuickBookSession sess)
		{
			// QBLogHelper.QBLogRequest("QuickBooksCustomerOps.ModCustomers \t  => ");

			try
			{
				var customers = new List<Guid>();
				if (sess.GetProperty(ModRequestDbList.CustomersModLists.ToString()) != null)
				{
					var lists = sess.GetProperty(ModRequestDbList.CustomersModLists.ToString());
					var dbList = (List<Guid>)lists;

					customers = dbList.Take(100).Distinct().ToList();

					var updatedList = dbList.Except(customers).ToList();
					sess.SetProperty(ModRequestDbList.CustomersModLists.ToString(), updatedList);
				}

				var customerModRqXML = (string)await BuildRqXML(sess, customers);
				return customerModRqXML;
			}
			catch (Exception exp)
			{

				//return empty request so flow continue
				return EmptyRequest();
			}
		}

		private async Task<string> BuildRqXML(QuickBookSession sess, List<Guid> customers, bool isQuery = false)
		{
			bool isCustomerAddRequest = sess.GetProperty("controllerState") == null ||
										sess.GetProperty("controllerState").ToString() == "CustomerAddRq" || sess.GetProperty("lastControllerState").ToString() == "CustomerAddRs";


			var getAllCustomers = await _partnerRepository.GetAllListAsync(x => customers.Contains(x.Id));


			var requestSet = new XmlDocument();
			requestSet.AppendChild(requestSet.CreateXmlDeclaration("1.0", "utf-8", null));
			requestSet.AppendChild(requestSet.CreateProcessingInstruction("qbxml", "version=\"13.0\""));

			var qbXML = requestSet.CreateElement("QBXML");
			requestSet.AppendChild(qbXML);

			var qbXMLMsgsRq = requestSet.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "continueOnError");
			getAllCustomers = getAllCustomers.Distinct().ToList();
			//for querying Request
			if (isQuery)
			{
				BuildCustomerQueryRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);
				return requestSet.OuterXml;
			}

			BuildCustomerModRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);

			return requestSet.OuterXml;
		}



		private void BuildCustomerQueryRequest(List<Partner> customers, QuickBookSession sess, XmlElement qbXMLMsgsRq,
			ref XmlDocument requestSet)
		{
			var listIds = new List<UtilityClass>();
			var counter = 0;

			var customerunqiuelists = customers.Where(x => x.QuickBooksDesktopId != null && x.IsQBSynced == false).ToList().Distinct();
			foreach (var cust in customerunqiuelists)
				try
				{
					var custQueryReq = requestSet.CreateElement("CustomerQueryRq");
					qbXMLMsgsRq.AppendChild(custQueryReq);
					custQueryReq.SetAttribute("requestID", counter.ToString());

					var idsClass = new UtilityClass { RequestId = counter.ToString(), EntityID = cust.Id };
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
					if (childNode != null) qbXMLMsgsRq.RemoveChild(childNode);

					listIds.RemoveAll(x => x.RequestId == counter.ToString());
				}

			sess.SetProperty(CustomersIds, listIds);
		}

		private void BuildCustomerModRequest(List<Partner> partners, QuickBookSession sess, XmlElement qbXMLMsgsRq,
			ref XmlDocument requestSet)
		{
			try
			{
				var listIds = new List<UtilityClass>();

				var customersIdsList = GetQueryListIds(sess);
				var counter = 0;

				foreach (var partner in partners)
					try
					{

						var objIds = customersIdsList.FirstOrDefault(y => y.EntityID == partner.Id);
						if (objIds != null && !string.IsNullOrEmpty(objIds.EditSequence)
										   && partner.QuickBooksDesktopId == objIds.QbDesktopId
						) //as Edit sequence And QBID(ListID) is required for update in Qb desktop other wise it may cause exception in Qb and stop the process
						{

							var custModRq = requestSet.CreateElement("CustomerModRq");
							qbXMLMsgsRq.AppendChild(custModRq);
							custModRq.SetAttribute("requestID", counter.ToString());
							var custMod = requestSet.CreateElement("CustomerMod");
							custModRq.AppendChild(custMod);


							var idsClass = new UtilityClass { RequestId = counter.ToString(), EntityID = partner.Id };
							listIds.Add(idsClass);

							custMod.AppendChild(requestSet.CreateElement("ListID")).InnerText = partner?.QuickBooksDesktopId.Trim();
							custMod.AppendChild(requestSet.CreateElement("EditSequence")).InnerText = objIds.EditSequence;

							#region Names

							custMod.AppendChild(requestSet.CreateElement("Name")).InnerText = (partner.Name).ToCleanStringSub0(41);
							custMod.AppendChild(requestSet.CreateElement("IsActive")).InnerText = "true";

							custMod.AppendChild(requestSet.CreateElement("CompanyName")).InnerText =
								partner.FullName.ToCleanStringSub0(41);

							custMod.AppendChild(requestSet.CreateElement("FirstName")).InnerText =
								partner.Name.ToCleanStringSub0(25);

							custMod.AppendChild(requestSet.CreateElement("LastName")).InnerText = "";

							#endregion

							counter++;
						}

					}
					catch (Exception exp)
					{


						//remove the current Request from Xml tree as exception occured and may cause error in QB
						var qbXMLMsgsRsNodeList = requestSet.GetElementsByTagName("QBXMLMsgsRq");
						var childNode =
							qbXMLMsgsRsNodeList.Cast<XmlNode>().Select(x => x.LastChild).FirstOrDefault();
						if (childNode != null) qbXMLMsgsRq.RemoveChild(childNode);
						listIds.RemoveAll(x => x.RequestId == counter.ToString());
					}

				sess.SetProperty(CustomersIds, listIds);
			}
			catch (Exception exp)
			{

			}
		}

		#endregion

		public void UpdateCustomer(string response, QuickBookSession sess)
		{

			try
			{
				var outputXMLDoc = new XmlDocument();
				outputXMLDoc.LoadXml(response);
				var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerAddRs");
				qbXMLMsgsRsNodeList = qbXMLMsgsRsNodeList.Count > 0
					? qbXMLMsgsRsNodeList
					: outputXMLDoc.GetElementsByTagName("CustomerModRs");

				var customersIdsSess = sess.GetProperty(CustomersIds);
				var customersIdsList = (List<UtilityClass>)customersIdsSess;

				var count = qbXMLMsgsRsNodeList.Count;
				for (var x = 0; x < count; x++)
					try
					{
						var rsAttributes = qbXMLMsgsRsNodeList.Item(x).Attributes;
						var retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
						//var retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
						var retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;
						var retRequestID = rsAttributes.GetNamedItem("requestID").Value;

						//get the response node for detailed info  and a response contains max one childNode for "CustomerRet"
						var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(x).ChildNodes;

						var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);



						if (custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0).Name.Equals("CustomerRet") &&
							retStatusCode == "0" ||
							retStatusCode == "530"
						) //Record Updated 530 status mean record Exported But For Some Record Info Currently QB Not Providing Implementation Of it.
						{

							var custRetNodeList = custAddRsNodeList.Item(0).ChildNodes;
							var qbDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID")
								.Select(z => z.InnerText).FirstOrDefault();
							if (objIds != null)
								objIds.QbDesktopId = qbDesktopId;



							var getCustomer = _partnerRepository.FirstOrDefault(x => x.Id == objIds.EntityID);
							if (getCustomer != null && !string.IsNullOrEmpty(qbDesktopId))
							{
								getCustomer.IsQBSynced = true;
								getCustomer.QuickBooksDesktopId = qbDesktopId;
								_partnerRepository.UpdateAsync(getCustomer);
							}
						}

					}

					catch (Exception exp)
					{

						var rsAttributes = qbXMLMsgsRsNodeList.Item(x).Attributes;
						var retRequestID = rsAttributes.GetNamedItem("requestID").Value;
						var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);
						var custID = objIds != null ? Convert.ToInt32(objIds.EntityID) : 0;


					}

				////add Those ServiceAddress id to cache so later we can get it and update these address in Qb as Qb at a time add 99 ship to address for a customer
				//AddServicesToSess(sess, CustomersIdsList);
			}
			catch (Exception exp)
			{

			}

			sess.RemoveProperty(CustomersIds);
		}

		public void GetCustomersFromQuery(string response, QuickBookSession sess)
		{


			try
			{

				var listIds = GetQueryListIds(sess);
				var outputXMLDoc = new XmlDocument();
				outputXMLDoc.LoadXml(response);
				var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerQueryRs");

				var customersIdsSess = sess.GetProperty(CustomersIds);
				var customersIdsList = (List<UtilityClass>)customersIdsSess;

				var count = qbXMLMsgsRsNodeList.Count;
				for (var x = 0; x < count; x++)
					try
					{
						var rsAttributes = qbXMLMsgsRsNodeList.Item(x)?.Attributes;
						var retStatusCode = rsAttributes?.GetNamedItem("statusCode").Value;
						//var retStatusSeverity = rsAttributes?.GetNamedItem("statusSeverity").Value;
						//var retStatusMessage = rsAttributes?.GetNamedItem("statusMessage").Value;
						var retRequestID = rsAttributes?.GetNamedItem("requestID").Value;

						//get the response node for detailed info  and a response contains max one childNode for "CustomerRet"
						var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(x)?.ChildNodes;
						if (custAddRsNodeList != null &&
							(custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0)?.Name == "CustomerRet" &&
							 (retStatusCode == "0" || retStatusCode == "530")))
						{
							var custRetNodeList = custAddRsNodeList.Item(0)?.ChildNodes;
							var qbDesktopId = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "ListID")
								.Select(z => z.InnerText).FirstOrDefault();
							var editSequence = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "EditSequence")
								.Select(z => z.InnerText).FirstOrDefault();
							var name = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "FullName")
								.Select(z => z.InnerText).FirstOrDefault();

							var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);

							var idsClass = new UtilityClass
							{
								RequestId = x.ToString(),
								EntityID = objIds.EntityID,
								EditSequence = editSequence,
								QbDesktopId = qbDesktopId,
								Name = name
							};
							listIds.Add(idsClass);
						}
					}
					catch (Exception exp)
					{

					}


				//remove previous Ids and add new List and Edit sequence against those entity which are going to update
				sess.RemoveProperty(CustomersIds);
				SetQueryListIds(sess, listIds);

			}
			catch (Exception exp)
			{

			}


		}

		#endregion

		#region Importing Section

		public string QueryCustomersForImport(QuickBookSession sess)
		{


			try
			{
				var customerSess = sess.GetProperty(ImportRequestList.CustomerImport.ToString());
				var getForIterator = (UtilityClass)customerSess;

				var objForIterator = new UtilityClass();
				if (getForIterator == null)
				{
					objForIterator.Iterator = "Start";
					objForIterator.RequestId = "0";
				}
				else
				{
					objForIterator.Iterator = getForIterator.Iterator == "Start" ? "Continue" : getForIterator.Iterator;
					objForIterator.IteratorID = getForIterator.IteratorID;
					objForIterator.RequestId = (getForIterator.RequestId.ToInt0() + 1).ToString();
				}

				var requestSet = new XmlDocument();
				requestSet.AppendChild(requestSet.CreateXmlDeclaration("1.0", "utf-8", null));
				requestSet.AppendChild(requestSet.CreateProcessingInstruction("qbxml", "version=\"13.0\""));
				var qbXML = requestSet.CreateElement("QBXML");
				requestSet.AppendChild(qbXML);
				var qbXMLMsgsRq = requestSet.CreateElement("QBXMLMsgsRq");
				qbXML.AppendChild(qbXMLMsgsRq);
				qbXMLMsgsRq.SetAttribute("onError", "continueOnError");

				var custQueryReq = requestSet.CreateElement("CustomerQueryRq");
				qbXMLMsgsRq.AppendChild(custQueryReq);
				custQueryReq.SetAttribute("requestID", objForIterator.RequestId);
				custQueryReq.SetAttribute("iterator", objForIterator.Iterator);
				if (!string.IsNullOrEmpty(objForIterator.IteratorID))
					custQueryReq.SetAttribute("iteratorID", objForIterator.IteratorID);

				custQueryReq.AppendChild(requestSet.CreateElement("MaxReturned")).InnerText = MaxReturnedRecordSize;
				custQueryReq.AppendChild(requestSet.CreateElement("ActiveStatus")).InnerText = "ActiveOnly";


				sess.SetProperty(ImportRequestList.CustomerImport.ToString(), objForIterator);

				return requestSet.OuterXml;
			}
			catch (Exception exp)
			{
				//return empty request so flow continue
				return EmptyRequest();
			}
		}

		public async Task GetCustomersFromQueryImportAsync(string response, QuickBookSession sess)
		{
			//response.QBLogRequest("QuickBooksCustomerOps.GetCustomersFromQueryImport \t  => ");

			try
			{
				var outputXMLDoc = new XmlDocument();
				outputXMLDoc.LoadXml(response);
				var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerQueryRs");


				if (qbXMLMsgsRsNodeList.Count > 0)
					try
					{
						var rsAttributes = qbXMLMsgsRsNodeList.Item(0)?.Attributes;
						if (rsAttributes != null)
						{
							var retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
							//var retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
							//var retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;
							//var retRequestID = rsAttributes.GetNamedItem("requestID").Value;
							var retIteratorRemainingCount = rsAttributes.GetNamedItem("iteratorRemainingCount").Value;
							var retIteratorID = rsAttributes.GetNamedItem("iteratorID").Value;

							var customerSess = sess.GetProperty(ImportRequestList.CustomerImport.ToString());
							var objForIterator = (UtilityClass)customerSess;

							objForIterator.IteratorRemainingCount = retIteratorRemainingCount;
							objForIterator.IteratorID = retIteratorID;

							//get the response node for detailed info  and a response contains max one childNode for "CustomerQueryRs" list in iterator Query
							var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(0)?.ChildNodes;
							if (custAddRsNodeList != null)
							{
								var length = custAddRsNodeList.Count;


								if (objForIterator.Iterator == "Start")
								{
									var total = retIteratorRemainingCount.ToDecimal() + length;

								}

								var TenantId = (int)sess.GetProperty("TenantId");
								_unitOfWorkManager.Current.SetTenantId(TenantId);
								for (var i = 0; i < length; i++)
									try
									{
										if (custAddRsNodeList.Count > 1 && custAddRsNodeList.Item(i)?.Name == "CustomerRet" &&
											(retStatusCode == "0" || retStatusCode == "530"))
										{
											var c = GetCount(sess);

											var custRetNodeList = custAddRsNodeList.Item(i)?.ChildNodes;
											var qbDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID").Select(z => z.InnerText).FirstOrDefault();
											var CheckIfCustomerExists = await _partnerRepository.FirstOrDefaultAsync(x => x.QuickBooksDesktopId == qbDesktopId);
											if (CheckIfCustomerExists == null)
											{
												await MapQBCustomerToPartner(custRetNodeList, CheckIfCustomerExists, true);
											}
											else
											{
												var timeModified = custRetNodeList.Cast<XmlNode>().ToList().Where(y => y.Name == "TimeModified").Select(z => z.InnerText).FirstOrDefault().FromTimeStampToDate();
												if (CheckIfCustomerExists.LastModificationTime.HasValue && CheckIfCustomerExists.LastModificationTime != timeModified)
												{
													if (timeModified > CheckIfCustomerExists.LastModificationTime)
													{
														await MapQBCustomerToPartner(custRetNodeList, CheckIfCustomerExists, false);
													}
													else
													{
														CheckIfCustomerExists.IsQBSynced = false;
														await _partnerRepository.UpdateAsync(CheckIfCustomerExists);
														await _unitOfWorkManager.Current.SaveChangesAsync();
													}
												}
											}

										}
									}
									catch (Exception exp)
									{

									}




							}

							if (retIteratorRemainingCount.ToInt0() > 0)
								sess.SetProperty(ImportRequestList.CustomerImport.ToString(), objForIterator);
							else
								sess.RemoveProperty(ImportRequestList.CustomerImport.ToString());
						}
					}
					catch (Exception exp)
					{

					}
			}
			catch (Exception exp)
			{

			}
		}

		private async Task MapQBCustomerToPartner(XmlNodeList custRetNodeList, Partner partner, bool IsNewCustomer)
		{
			var custRetNode = custRetNodeList.Cast<XmlNode>();
			if (IsNewCustomer)
				partner = new Partner();

			var xmlNodes = custRetNode.ToList();

			//var qbDesktopId = xmlNodes.Where(y => y.Name == "ListID").Select(z => z.InnerText).FirstOrDefault();
			var companyName = xmlNodes.Where(y => y.Name == "CompanyName").Select(z => z.InnerText).FirstOrDefault();
			var name = xmlNodes.Where(y => y.Name == "Name").Select(z => z.InnerText).FirstOrDefault();
			var fullName = xmlNodes.Where(y => y.Name == "FullName").Select(z => z.InnerText).FirstOrDefault();
			var firstName = xmlNodes.Where(y => y.Name == "FirstName").Select(z => z.InnerText).FirstOrDefault();
			var lastName = xmlNodes.Where(y => y.Name == "LastName").Select(z => z.InnerText).FirstOrDefault();
			var phone = xmlNodes.Where(y => y.Name == "Phone").Select(z => z.InnerText).FirstOrDefault();
			var altPhone = xmlNodes.Where(y => y.Name == "AltPhone").Select(z => z.InnerText).FirstOrDefault();
			var fax = xmlNodes.Where(y => y.Name == "Fax").Select(z => z.InnerText).FirstOrDefault();
			var email = xmlNodes.Where(y => y.Name == "Email").Select(z => z.InnerText).FirstOrDefault();
			var timeCreated = xmlNodes.Where(y => y.Name == "TimeCreated").Select(z => z.InnerText).FirstOrDefault();
			var timeModified = xmlNodes.Where(y => y.Name == "TimeModified").Select(z => z.InnerText).FirstOrDefault();


			partner.QuickBooksDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID").Select(z => z.InnerText).FirstOrDefault();
			partner.FullName = !string.IsNullOrEmpty(companyName) ? companyName.Trim() : "";

			partner.FullName = fullName;
			partner.Name = !string.IsNullOrEmpty(name) ? name : firstName + " " + lastName;
			partner.LastModificationTime = timeModified.FromTimeStampToDate();


			if (IsNewCustomer)
				await _partnerRepository.InsertAsync(partner);
			else
				await _partnerRepository.UpdateAsync(partner);


			await _unitOfWorkManager.Current.SaveChangesAsync();
		}

		#endregion


	}
}
