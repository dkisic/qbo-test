using Abp.Dependency;
using Abp.Domain.Repositories;
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

	public class QuickBooksCustomerOps : BaseClass, ITransientDependency
	{
		private const string CustomersIds = "CustomersIds";
		private readonly IRepository<Partner, Guid> _partnerRepository;

		public QuickBooksCustomerOps(IRepository<Partner, Guid> partnerRepository)
		{
			_partnerRepository = partnerRepository;
		}


		//Constructor

		//#region Export Section

		//#region Request

		//public string AddCustomers(QuickBookSession sess)
		//{
		//	// QBLogHelper.QBLogRequest("QuickBooksCustomerOps.AddCustomers \t  => ");

		//	try
		//	{
		//		var _customerServices = new CustomerService();
		//		List<int> customers;

		//		if (sess.GetProperty(AddRequestDbList.CustomersLists.ToString()) == null)
		//		{
		//			List<int> customersids;

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

		//public string QueryCustomers(QuickBookSession sess)
		//{
		//	// QBLogHelper.QBLogRequest("QuickBooksCustomerOps.QueryCustomers \t  => ");

		//	try
		//	{
		//		List<int> customers;
		//		if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) == null)
		//		{
		//			List<int> customersids;

		//			//GetAllCustomersForQBDesktopUpdate


		//			sess.SetProperty(ModRequestDbList.CustomersModLists.ToString(),
		//				customersids); //adding it to session to get again it from it to mod in Qb
		//			customers = customersids.Take(100).Distinct().ToList();
		//			customersids = customersids.Except(customers).ToList();
		//			sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), customersids);
		//		}
		//		else
		//		{
		//			var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
		//			var dbList = (List<int>)lists;
		//			customers = dbList.Take(100).Distinct().ToList();

		//			var updatedList = dbList.Except(customers).ToList();
		//			sess.SetProperty(QueryRequestDbList.CustomersQueryLists.ToString(), updatedList);
		//		}

		//		var customerModRqXML = BuildRqXML(sess, customers, true);
		//		return customerModRqXML;
		//	}
		//	catch (Exception exp)
		//	{

		//		//return empty request so flow continue
		//		return EmptyRequest();
		//	}
		//}

		//public string ModCustomers(QuickBookSession sess)
		//{
		//	// QBLogHelper.QBLogRequest("QuickBooksCustomerOps.ModCustomers \t  => ");

		//	try
		//	{
		//		var customers = new List<int>();
		//		if (sess.GetProperty(ModRequestDbList.CustomersModLists.ToString()) != null)
		//		{
		//			var lists = sess.GetProperty(ModRequestDbList.CustomersModLists.ToString());
		//			var dbList = (List<int>)lists;

		//			customers = dbList.Take(100).Distinct().ToList();

		//			var updatedList = dbList.Except(customers).ToList();
		//			sess.SetProperty(ModRequestDbList.CustomersModLists.ToString(), updatedList);
		//		}

		//		var customerModRqXML = BuildRqXML(sess, customers);
		//		return customerModRqXML;
		//	}
		//	catch (Exception exp)
		//	{

		//		//return empty request so flow continue
		//		return EmptyRequest();
		//	}
		//}

		//private string BuildRqXML(QuickBookSession sess, List<int> customers, bool isQuery = false)
		//{
		//	bool isCustomerAddRequest = sess.GetProperty("controllerState") == null ||
		//								sess.GetProperty("controllerState").ToString() == "CustomerAddRq" || sess.GetProperty("lastControllerState").ToString() == "CustomerAddRs";


		//	DB<Customer> mydb = new DB<Customer>();
		//	var getAllCustomers = mydb.Repository.GetQueryable().Include("CustomerAddresses").Where(x => customers.Contains(x.CustomerID)).Distinct().ToList();

		//	getAllCustomers.Select(t => t.CustomerID).QBLogRequest($"QuickBooksCustomerOps.BuildRqXML isQuery:{isQuery} isCustomerAddRequest:{isCustomerAddRequest} CustomerIDs:::");


		//	var getDuplicatcut = sess.GetProperty(AddRequestDbList.CustomersDupLists.ToString());
		//	List<int> duplist = getDuplicatcut != null ? (List<int>)getDuplicatcut : new List<int>();
		//	var duplistDb = new List<Customer>();
		//	if (getDuplicatcut != null && duplist.Count > 0)
		//	{
		//		duplistDb = mydb.Repository.GetQueryable().Include("CustomerAddresses").Where(x => duplist.Contains(x.CustomerID)).ToList();

		//	}
		//	foreach (var item in duplistDb)
		//	{
		//		var cust = AddRandomNameWihCust(item);
		//		getAllCustomers.Add(cust);
		//	}

		//	sess.RemoveProperty(AddRequestDbList.CustomersDupLists.ToString());

		//	var requestSet = new XmlDocument();
		//	requestSet.AppendChild(requestSet.CreateXmlDeclaration("1.0", "utf-8", null));
		//	requestSet.AppendChild(requestSet.CreateProcessingInstruction("qbxml", "version=\"13.0\""));

		//	var qbXML = requestSet.CreateElement("QBXML");
		//	requestSet.AppendChild(qbXML);

		//	var qbXMLMsgsRq = requestSet.CreateElement("QBXMLMsgsRq");
		//	qbXML.AppendChild(qbXMLMsgsRq);
		//	qbXMLMsgsRq.SetAttribute("onError", "continueOnError");
		//	getAllCustomers = getAllCustomers.Distinct().ToList();
		//	//for querying Request
		//	if (isQuery)
		//	{
		//		BuildCustomerQueryRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);
		//		return requestSet.OuterXml;
		//	}

		//	//for Add and Mod
		//	if (isCustomerAddRequest)
		//		BuildCustomerRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);
		//	else
		//	{
		//		var getDuplicatModcust = sess.GetProperty(ModRequestDbList.CustomersDupModLists.ToString());
		//		List<int> dupModlist = getDuplicatModcust != null ? (List<int>)getDuplicatModcust : new List<int>();
		//		var dupModlistDb = new List<Customer>();
		//		if (getDuplicatModcust != null && duplist.Count > 0)
		//		{
		//			dupModlistDb = mydb.Repository.GetQueryable().Include("CustomerAddresses").Where(x => duplist.Contains(x.CustomerID)).ToList();

		//		}
		//		foreach (var item in dupModlistDb)
		//		{
		//			var cust = AddRandomNameWihCust(item);
		//			getAllCustomers.Add(cust);
		//		}
		//		getAllCustomers = getAllCustomers.Distinct().ToList();
		//		sess.RemoveProperty(ModRequestDbList.CustomersDupModLists.ToString());
		//		BuildCustomerModRequest(getAllCustomers, sess, qbXMLMsgsRq, ref requestSet);
		//	}
		//	return requestSet.OuterXml;
		//}

		//private void BuildCustomerRequest(List<Customer> customers, QuickBookSession sess, XmlElement qbXMLMsgsRq,
		//	ref XmlDocument requestSet)
		//{
		//	var listIds = new List<UtilityClass>();
		//	var counter = 0;

		//	var customerunqiuelists = customers.Where(x => x.QBDesktopId == null && x.IsQBSynced == false).ToList().Distinct();
		//	foreach (var cust in customerunqiuelists)
		//		try
		//		{

		//			var customerAddresses = cust.CustomerAddresses.Where(x => x.CustomerID == cust.CustomerID).ToList();
		//			var servicestateName = "";
		//			var billingstateName = "";
		//			var billingFirstName = "";
		//			var billingaddress1 = "";
		//			var billingaddress = "";
		//			var billingaddress2 = "";
		//			var billingcity = "";
		//			var billingzip = "";

		//			if (servicestateid != null)
		//			{
		//				servicestateName = _stateServices.GetStateAbbrivationByID(Convert.ToInt32(servicestateid)); //for the service address

		//			}

		//			if (billingstateid != 0 && billingstateid != null)
		//			{
		//				billingstateName = _stateServices.GetStateAbbrivationByID(Convert.ToInt32(billingstateid)); //for the service address
		//			}
		//			var randomName = Randomizer.RandomNumberOfLetters(5);
		//			var randomNameExtra = Randomizer.RandomNumberOfLetters(3);
		//			var custAddRq = requestSet.CreateElement("CustomerAddRq");
		//			qbXMLMsgsRq.AppendChild(custAddRq);
		//			custAddRq.SetAttribute("requestID", counter.ToString());

		//			var custAdd = requestSet.CreateElement("CustomerAdd");
		//			custAddRq.AppendChild(custAdd);

		//			var idsClass = new UtilityClass { RequestId = counter.ToString(), EntityID = cust.CustomerID };
		//			listIds.Add(idsClass);

		//			#region Names
		//			var custName = "";
		//			var firstName = "";
		//			var lastName = "";

		//			if (string.IsNullOrEmpty(cust.FullName.Trim()))
		//			{
		//				if (cust.CompanyName != null) custName = cust.CompanyName.Trim();
		//				if (string.IsNullOrEmpty(cust.FirstName))
		//				{
		//					if (cust.Title1 != null) firstName = cust.Title1.Trim();
		//					if (cust.Title1LastName != null)
		//						lastName = cust.Title1LastName.Trim().ToCleanStringQB();
		//				}
		//				else
		//				{
		//					firstName = cust.FirstName.Trim();
		//					if (cust.LastName != null) lastName = cust.LastName.Trim();
		//				}
		//			}
		//			else
		//			{
		//				custName = cust.FullName.Trim().ToCleanStringQB();
		//				firstName = cust.FirstName.Trim().ToCleanStringQB();
		//				lastName = cust.LastName.Trim().ToCleanStringQB();
		//			}

		//			if (string.IsNullOrEmpty(custName.Trim()))
		//			{
		//				if (cust.Title1 != null)
		//				{
		//					firstName = cust.Title1.Trim();
		//					if (cust.Title1LastName != null)
		//					{
		//						custName = cust.Title1.Trim().ToCleanStringQB() + " " + cust.Title1LastName.Trim().ToCleanStringQB();
		//						lastName = cust.Title1LastName.Trim().ToCleanStringQB();
		//					}

		//				}
		//			}
		//			if (string.IsNullOrEmpty(custName.Trim()))
		//			{
		//				var customerAddress = customerAddresses.FirstOrDefault();
		//				if (customerAddress != null && !string.IsNullOrEmpty(customerAddress.CompanyFirstName1))
		//				{
		//					if (!string.IsNullOrEmpty(customerAddress.CompanyLastName1))
		//					{
		//						custName = (customerAddress.CompanyFirstName1 + " " + customerAddress.CompanyLastName1).Trim().ToCleanStringSub0(41);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase).Trim() + " " + Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//					}
		//					else
		//					{
		//						custName = (customerAddress.CompanyFirstName1);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase).Trim() + " " + Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//					}

		//					firstName = (customerAddress.CompanyFirstName1).Trim().ToCleanStringSub0(25);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);

		//					if (!string.IsNullOrEmpty(customerAddress.CompanyLastName1))
		//					{
		//						lastName = (customerAddress.CompanyLastName1).Trim().ToCleanStringSub0(25);//Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//					}

		//				}
		//			}

		//			//custName = Regex.Replace(custName.Trim(), "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//			custName = custName == null ? "" : custName.Trim().ToCleanStringSub0(41);
		//			if (string.IsNullOrEmpty(custName) || string.IsNullOrWhiteSpace(custName))
		//			{
		//				if (cust.CompanyName != null)
		//					custName = cust.CompanyName.Trim();// Regex.Replace(t.CompanyName.Trim(), "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase); //t.CompanyName; 
		//				custName = custName == null ? "" : custName.Trim().ToCleanStringSub0(41);
		//				if (string.IsNullOrEmpty(custName.Trim()))
		//					custName = "NONAME_" + Randomizer.RandomNumberOfLettersAll(5).Trim().ToCleanStringSub0(41);
		//			}
		//			custAdd.AppendChild(requestSet.CreateElement("Name")).InnerText = (custName).ToCleanStringSub0(41);
		//			custAdd.AppendChild(requestSet.CreateElement("IsActive")).InnerText = "true";
		//			cust.CompanyName = !string.IsNullOrEmpty(cust.CompanyName) || !string.IsNullOrWhiteSpace(cust.CompanyName) ? (cust.CompanyName).ToCleanStringSub0(41) : custName;
		//			custAdd.AppendChild(requestSet.CreateElement("CompanyName")).InnerText =
		//			   cust.CompanyName.ToCleanStringSub0(41);

		//			if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrWhiteSpace(firstName))
		//				custAdd.AppendChild(requestSet.CreateElement("FirstName")).InnerText =
		//					firstName.ToCleanStringSub0(25);

		//			if (!string.IsNullOrEmpty(lastName) || !string.IsNullOrWhiteSpace(lastName))
		//				custAdd.AppendChild(requestSet.CreateElement("LastName")).InnerText =
		//					lastName.ToCleanStringSub0(25);
		//			//else
		//			//{
		//			//    custAdd.AppendChild(requestSet.CreateElement("LastName")).InnerText = custName.ToCleanStringSub0(25);
		//			//}

		//			#endregion

		//			#region billing Address

		//			var billAddress = requestSet.CreateElement("BillAddress");
		//			custAdd.AppendChild(billAddress);

		//			if (!string.IsNullOrEmpty(cust.CompanyAddress) || !string.IsNullOrWhiteSpace(cust.CompanyAddress))
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText =
		//					(cust.CompanyAddress).ToCleanStringSub0(41);
		//			}
		//			else
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText =
		//												(billingFirstName ?? "").ToCleanStringSub0(41);
		//				if (billingaddress != null)
		//				{
		//					billAddress.AppendChild(requestSet.CreateElement("Addr2")).InnerText =
		//					  (billingaddress ?? "").ToCleanStringSub0(41);
		//				}
		//				else
		//				{
		//					billAddress.AppendChild(requestSet.CreateElement("Addr2")).InnerText =
		//												  (billingaddress1 ?? "").ToCleanStringSub0(41);
		//				}

		//				billAddress.AppendChild(requestSet.CreateElement("Addr3")).InnerText =
		//											(billingaddress2 ?? "").ToCleanStringSub0(41);
		//			}


		//			if (!string.IsNullOrEmpty(cust.CompanyCity) || !string.IsNullOrWhiteSpace(cust.CompanyCity))
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("City")).InnerText =
		//					  ((cust.CompanyCity ?? "")).ToCleanStringSub0(31);
		//			}
		//			else
		//			{

		//				billAddress.AppendChild(requestSet.CreateElement("City")).InnerText =
		//					  (billingcity ?? "").ToCleanStringSub0(31);
		//			}

		//			if (!string.IsNullOrEmpty(billingstateName) || !string.IsNullOrWhiteSpace(billingstateName))
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("State")).InnerText =
		//					  ((billingstateName ?? "")).ToCleanStringSub0(31);
		//			}
		//			if (!string.IsNullOrEmpty(cust.CompanyZip) || !string.IsNullOrWhiteSpace(cust.CompanyZip))
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText =
		//					cust.CompanyZip.ToCleanStringSub0(13);
		//			}
		//			else
		//			{
		//				billAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText =
		//				   (billingzip ?? "").ToCleanStringSub0(13);
		//			}


		//			billAddress.AppendChild(requestSet.CreateElement("Country")).InnerText = "USA";

		//			#endregion

		//			#region Shipping Address

		//			//We can only sync 50 Ship To address in a single request
		//			var serviceAddressToSync = customerAddresses.Take(49).ToList();
		//			idsClass.ServiceAddressListId =
		//				customerAddresses.Except(serviceAddressToSync).Select(x => x.AddressID).ToList();

		//			var addressList = new List<ServiceAddress>();
		//			foreach (var customerAddress in serviceAddressToSync)
		//			{
		//				var addName = $"{customerAddress.CompanyFirstName1} {customerAddress.CompanyLastName1}]";
		//				if (addressList.Any(t => t.Name == addName))
		//					addName = $"{customerAddress.CompanyFirstName1} {customerAddress.CompanyLastName1} - {customerAddress.AddressID}]";

		//				addressList.Add(new ServiceAddress
		//				{
		//					Name = addName,
		//					Addr1 = customerAddress.Address,
		//					City = customerAddress.City ?? string.Empty,
		//					PostalCode = customerAddress.Zip ?? string.Empty,
		//					AddressId = customerAddress.AddressID.ToString()
		//				});
		//			}

		//			foreach (var customerAddress in addressList)
		//			{
		//				var shipToAddress = requestSet.CreateElement("ShipToAddress");
		//				custAdd.AppendChild(shipToAddress);

		//				shipToAddress.AppendChild(requestSet.CreateElement("Name")).InnerText = customerAddress.Name.ToCleanStringSub0(41);

		//				if (!string.IsNullOrEmpty(customerAddress.Addr1))
		//				{
		//					shipToAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText =
		//						(customerAddress.Addr1).ToCleanStringSub0(41);
		//				}


		//				if (!string.IsNullOrEmpty(customerAddress.City))
		//				{
		//					shipToAddress.AppendChild(requestSet.CreateElement("City")).InnerText =
		//						((customerAddress.City.Trim())).ToCleanStringSub0(31);
		//				}

		//				if (!string.IsNullOrEmpty(servicestateName))
		//				{
		//					shipToAddress.AppendChild(requestSet.CreateElement("State")).InnerText =
		//						((servicestateName ?? "")).ToCleanStringSub0(31);
		//				}

		//				if (!string.IsNullOrEmpty(customerAddress.PostalCode))
		//				{
		//					shipToAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText = customerAddress.PostalCode.Trim().ToCleanStringSub0(13);
		//				}


		//				shipToAddress.AppendChild(requestSet.CreateElement("Country")).InnerText = "USA";
		//			}

		//			#endregion

		//			#region contact

		//			if (!string.IsNullOrEmpty(cust.compPrimaryContactNo) || !string.IsNullOrWhiteSpace(cust.compPrimaryContactNo))
		//			{
		//				custAdd.AppendChild(requestSet.CreateElement("Phone")).InnerText =
		//					cust.compPrimaryContactNo.CleanPhoneNo();
		//			}


		//			if (!string.IsNullOrEmpty(cust.CompanyEmail) && cust.CompanyEmail.Trim() != "N/A" && cust.CompanyEmail.Length > 7 &&
		//				!cust.CompanyEmail.Contains("-"))
		//				if (Regex.IsMatch(cust.CompanyEmail,
		//					@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
		//					RegexOptions.IgnoreCase))
		//				{
		//					custAdd.AppendChild(requestSet.CreateElement("Email")).InnerText =
		//						cust.CompanyEmail.Trim().ToLowerInvariant();
		//				}


		//			#endregion

		//			//custAdd.AppendChild(requestSet.CreateElement("ResaleNumber")).InnerText = cust.CustomerID.ToString();
		//			//custAdd.AppendChild(requestSet.CreateElement("AccountNumber")).InnerText = cust.CustomerID.ToString();
		//			custAdd.AppendChild(requestSet.CreateElement("Notes")).InnerText = "SkyBoss ID " + cust.CustomerID;
		//			counter++;
		//		}
		//		catch (Exception exp)
		//		{
		//			//exp.QBLogRequest("QuickBooksCustomerOps.BuildCustomerRequest \t  => ");
		//			new QbLogService().Error("QuickBooksCustomerOps.BuildCustomerRequest \t  => " + exp.Message + " => CustomerID: " + cust.CustomerID, exp);
		//			UpdateStatus(sess.GetUsername(),
		//				$"Customer ID: {cust.CustomerID.ToString()}, Message:{exp.Message} \n");

		//			//remove the current Request from Xml tree as exception occured and may cause error in QB
		//			var qbXMLMsgsRsNodeList = requestSet.GetElementsByTagName("QBXMLMsgsRq");
		//			var childNode =
		//				qbXMLMsgsRsNodeList.Cast<XmlNode>().Select(x => x.LastChild).FirstOrDefault();
		//			if (childNode != null) qbXMLMsgsRq.RemoveChild(childNode);

		//			listIds.RemoveAll(x => x.RequestId == counter.ToString());
		//		}

		//	sess.SetProperty(CustomersIds, listIds);
		//}

		//private void BuildCustomerQueryRequest(List<Customer> customers, QuickBookSession sess, XmlElement qbXMLMsgsRq,
		//	ref XmlDocument requestSet)
		//{
		//	var listIds = new List<UtilityClass>();
		//	var counter = 0;

		//	var customerunqiuelists = customers.Where(x => x.QBDesktopId != null && x.IsQBSynced == false).ToList().Distinct();
		//	foreach (var cust in customerunqiuelists)
		//		try
		//		{
		//			var custQueryReq = requestSet.CreateElement("CustomerQueryRq");
		//			qbXMLMsgsRq.AppendChild(custQueryReq);
		//			custQueryReq.SetAttribute("requestID", counter.ToString());

		//			var idsClass = new UtilityClass { RequestId = counter.ToString(), EntityID = cust.CustomerID };
		//			listIds.Add(idsClass);

		//			custQueryReq.AppendChild(requestSet.CreateElement("ListID")).InnerText = cust.QBDesktopId?.Trim();
		//			counter++;
		//		}
		//		catch (Exception exp)
		//		{
		//			//exp.QBLogRequest("QuickBooksCustomerOps.BuildCustomerQueryRequest \t  => ");
		//			new QbLogService().Error("QuickBooksCustomerOps.BuildCustomerQueryRequest \t  => " + exp.Message + " => CustomerID: " + cust.CustomerID, exp);

		//			//remove the current Request from Xml tree as exception occured and may cause error in QB
		//			var qbXMLMsgsRsNodeList = requestSet.GetElementsByTagName("QBXMLMsgsRq");
		//			var childNode =
		//				qbXMLMsgsRsNodeList.Cast<XmlNode>().Select(x => x.LastChild).FirstOrDefault();
		//			if (childNode != null) qbXMLMsgsRq.RemoveChild(childNode);

		//			listIds.RemoveAll(x => x.RequestId == counter.ToString());
		//		}

		//	sess.SetProperty(CustomersIds, listIds);
		//}

		//private void BuildCustomerModRequest(List<Customer> customers, QuickBookSession sess, XmlElement qbXMLMsgsRq,
		//	ref XmlDocument requestSet)
		//{
		//	try
		//	{
		//		var listIds = new List<UtilityClass>();
		//		var _stateServices = new StateService();
		//		DB<WorkOrder> mydb = new DB<WorkOrder>();
		//		DB<WorkOrderPayment> mydb1 = new DB<WorkOrderPayment>();
		//		var customersIdsList = GetQueryListIds(sess);
		//		var counter = 0;
		//		var customerunqiuelists = customers.Where(x => x.QBDesktopId != null && x.IsQBSynced == false).ToList().Distinct();
		//		foreach (var cust in customerunqiuelists)
		//			try
		//			{

		//				var objIds = customersIdsList.FirstOrDefault(y => y.EntityID == cust.CustomerID);
		//				if (objIds != null && !string.IsNullOrEmpty(objIds.EditSequence)
		//								   && cust.QBDesktopId == objIds.QbDesktopId
		//				) //as Edit sequence And QBID(ListID) is required for update in Qb desktop other wise it may cause exception in Qb and stop the process
		//				{
		//					var customerAddresses = cust.CustomerAddresses.Where(x => x.CustomerID == cust.CustomerID).ToList();
		//					var servicestateName = "";
		//					var billingstateName = "";
		//					var billingFirstName = "";
		//					var billingaddress1 = "";
		//					var billingaddress = "";
		//					var billingaddress2 = "";
		//					var billingcity = "";
		//					var billingzip = "";
		//					var servicestateid = cust?.CustomerAddresses.Where(x => x.CustomerID == cust.CustomerID).Select(x => x.StateID).FirstOrDefault();
		//					var Workorderobj = mydb.Repository.GetQueryable().Include("WorkorderPayments").Where(x => x.CustomerID == cust.CustomerID)
		//					   .FirstOrDefault();
		//					var billingstateid = Workorderobj != null ? Workorderobj?.WorkOrderPayments.Where(x => x.BillingStateProvinceID != null)
		//						.Select(x => x.BillingStateProvinceID).FirstOrDefault() : 0;
		//					billingaddress1 = Workorderobj?.WorkOrderPayments.Select(x => x.BillingAddress1).FirstOrDefault();
		//					billingcity = Workorderobj?.WorkOrderPayments.Select(x => x.BillingCity).FirstOrDefault();
		//					billingzip = Workorderobj?.WorkOrderPayments.Select(x => x.BillingZipPostalCode).FirstOrDefault();
		//					billingFirstName = Workorderobj?.WorkOrderPayments.Select(x => x.BillingFirstName).FirstOrDefault();
		//					billingaddress2 = Workorderobj?.WorkOrderPayments.Select(x => x.BillingAddress2).FirstOrDefault();
		//					billingaddress = customerAddresses.Select(x => x.Address).FirstOrDefault();
		//					if (servicestateid != null)
		//					{
		//						servicestateName = _stateServices.GetStateAbbrivationByID(Convert.ToInt32(servicestateid)); //for the service address

		//					}

		//					if (billingstateid != 0 && billingstateid != null)
		//					{
		//						billingstateName = _stateServices.GetStateAbbrivationByID(Convert.ToInt32(billingstateid)); //for the billing address
		//					}
		//					var custModRq = requestSet.CreateElement("CustomerModRq");
		//					qbXMLMsgsRq.AppendChild(custModRq);
		//					custModRq.SetAttribute("requestID", counter.ToString());
		//					var randomName = Randomizer.RandomNumberOfLetters(5);
		//					var randomNameExtra = Randomizer.RandomNumberOfLetters(3);
		//					var custMod = requestSet.CreateElement("CustomerMod");
		//					custModRq.AppendChild(custMod);


		//					var idsClass = new UtilityClass { RequestId = counter.ToString(), EntityID = cust.CustomerID };
		//					listIds.Add(idsClass);

		//					custMod.AppendChild(requestSet.CreateElement("ListID")).InnerText = cust?.QBDesktopId.Trim();
		//					custMod.AppendChild(requestSet.CreateElement("EditSequence")).InnerText = objIds.EditSequence;

		//					#region Names

		//					var custName = "";
		//					var firstName = "";
		//					var lastName = "";

		//					if (string.IsNullOrEmpty(cust.FullName.Trim()))
		//					{
		//						if (cust.CompanyName != null) custName = cust.CompanyName.Trim();
		//						if (string.IsNullOrEmpty(cust.FirstName))
		//						{
		//							if (cust.Title1 != null) firstName = cust.Title1.Trim();
		//							if (cust.Title1LastName != null)
		//								lastName = cust.Title1LastName.Trim().ToCleanStringQB();
		//						}
		//						else
		//						{
		//							firstName = cust.FirstName.Trim();
		//							if (cust.LastName != null) lastName = cust.LastName.Trim();
		//						}
		//					}
		//					else
		//					{
		//						custName = cust.FullName.Trim().ToCleanStringQB();
		//						firstName = cust.FirstName.Trim().ToCleanStringQB();
		//						lastName = cust.LastName.Trim().ToCleanStringQB();
		//					}

		//					if (string.IsNullOrEmpty(custName.Trim()))
		//					{
		//						if (cust.Title1 != null)
		//						{
		//							firstName = cust.Title1.Trim();
		//							if (cust.Title1LastName != null)
		//							{
		//								custName = cust.Title1.Trim().ToCleanStringQB() + " " + cust.Title1LastName.Trim().ToCleanStringQB();
		//								lastName = cust.Title1LastName.Trim().ToCleanStringQB();
		//							}

		//						}
		//					}
		//					if (string.IsNullOrEmpty(custName.Trim()))
		//					{
		//						var customerAddress = customerAddresses.FirstOrDefault();
		//						if (customerAddress != null && !string.IsNullOrEmpty(customerAddress.CompanyFirstName1))
		//						{
		//							if (!string.IsNullOrEmpty(customerAddress.CompanyLastName1))
		//							{
		//								custName = (customerAddress.CompanyFirstName1 + " " + customerAddress.CompanyLastName1).Trim().ToCleanStringSub0(41);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase).Trim() + " " + Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//							}
		//							else
		//							{
		//								custName = (customerAddress.CompanyFirstName1);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase).Trim() + " " + Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//							}

		//							firstName = (customerAddress.CompanyFirstName1).Trim().ToCleanStringSub0(25);//Regex.Replace(customerAddress.CompanyFirstName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);

		//							if (!string.IsNullOrEmpty(customerAddress.CompanyLastName1))
		//							{
		//								lastName = (customerAddress.CompanyLastName1).Trim().ToCleanStringSub0(25);//Regex.Replace(customerAddress.CompanyLastName1 ?? "", "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//							}

		//						}
		//					}
		//					//custName = Regex.Replace(custName.Trim(), "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase);
		//					custName = custName == null ? "" : custName.Trim().ToCleanStringSub0(41);
		//					if (string.IsNullOrEmpty(custName) || string.IsNullOrWhiteSpace(custName))
		//					{
		//						if (cust.CompanyName != null)
		//							custName = cust.CompanyName.Trim();// Regex.Replace(t.CompanyName.Trim(), "[^a-zA-Z' ]", "", RegexOptions.IgnoreCase); //t.CompanyName; 
		//						custName = custName == null ? "" : custName.Trim().ToCleanStringSub0(41);
		//						if (string.IsNullOrEmpty(custName.Trim()))
		//							custName = "NONAME_" + Randomizer.RandomNumberOfLettersAll(5).Trim().ToCleanStringSub0(41);
		//					}
		//					custMod.AppendChild(requestSet.CreateElement("Name")).InnerText = (custName).ToCleanStringSub0(41);
		//					custMod.AppendChild(requestSet.CreateElement("IsActive")).InnerText = "true";
		//					cust.CompanyName = !string.IsNullOrEmpty(cust.CompanyName) || !string.IsNullOrWhiteSpace(cust.CompanyName) ? (cust.CompanyName).ToCleanStringSub0(41) : custName;
		//					custMod.AppendChild(requestSet.CreateElement("CompanyName")).InnerText =
		//						cust.CompanyName.ToCleanStringSub0(41);

		//					if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrWhiteSpace(firstName))
		//					{
		//						custMod.AppendChild(requestSet.CreateElement("FirstName")).InnerText =
		//							firstName.ToCleanStringSub0(25);
		//					}



		//					if (!string.IsNullOrEmpty(lastName) || !string.IsNullOrWhiteSpace(lastName))
		//					{
		//						custMod.AppendChild(requestSet.CreateElement("LastName")).InnerText =
		//							lastName.ToCleanStringSub0(25);
		//					}


		//					//else
		//					//{
		//					//    CustMod.AppendChild(requestSet.CreateElement("LastName")).InnerText = custName.ToCleanStringSub0(25);
		//					//}

		//					#endregion

		//					#region billing Address

		//					var billAddress = requestSet.CreateElement("BillAddress");
		//					custMod.AppendChild(billAddress);

		//					if (!string.IsNullOrEmpty(cust.CompanyAddress) || !string.IsNullOrWhiteSpace(cust.CompanyAddress))
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText =
		//							(cust.CompanyAddress).ToCleanStringSub0(41);
		//					}
		//					else
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText =
		//														(billingFirstName ?? "").ToCleanStringSub0(41);
		//						if (billingaddress != null)
		//						{
		//							billAddress.AppendChild(requestSet.CreateElement("Addr2")).InnerText =
		//							  (billingaddress ?? "").ToCleanStringSub0(41);
		//						}
		//						else
		//						{
		//							billAddress.AppendChild(requestSet.CreateElement("Addr2")).InnerText =
		//														  (billingaddress1 ?? "").ToCleanStringSub0(41);
		//						}

		//						billAddress.AppendChild(requestSet.CreateElement("Addr3")).InnerText =
		//													(billingaddress2 ?? "").ToCleanStringSub0(41);
		//					}


		//					if (!string.IsNullOrEmpty(cust.CompanyCity) || !string.IsNullOrWhiteSpace(cust.CompanyCity))
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("City")).InnerText =
		//							  ((cust.CompanyCity)).ToCleanStringSub0(31);
		//					}
		//					else
		//					{

		//						billAddress.AppendChild(requestSet.CreateElement("City")).InnerText =
		//							  (billingcity ?? "").ToCleanStringSub0(31);
		//					}
		//					if (!string.IsNullOrEmpty(billingstateName) || !string.IsNullOrWhiteSpace(billingstateName))
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("State")).InnerText =
		//							 ((billingstateName ?? "")).ToCleanStringSub0(31);
		//					}
		//					if (!string.IsNullOrEmpty(cust.CompanyZip) || !string.IsNullOrWhiteSpace(cust.CompanyZip))
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText =
		//							cust.CompanyZip.ToCleanStringSub0(13);
		//					}
		//					else
		//					{
		//						billAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText =
		//						(billingzip ?? "").ToCleanStringSub0(13);
		//					}

		//					billAddress.AppendChild(requestSet.CreateElement("Country")).InnerText = "USA";

		//					#endregion

		//					#region Shipping Address
		//					// Commented because of the Crashing Error, when we sent the same address again, it Crashes the Quick Books

		//					//var serviceAddressToSync = customerAddresses.Take(49).ToList();
		//					//idsClass.ServiceAddressListId = customerAddresses.Except(serviceAddressToSync)
		//					//	.Select(x => x.AddressID).ToList();

		//					//var addressList = new List<ServiceAddress>();
		//					//foreach (var customerAddress in serviceAddressToSync)
		//					//{
		//					//	var addName = $"{customerAddress.CompanyFirstName1} {customerAddress.CompanyLastName1}]";
		//					//	if (addressList.Any(t => t.Name == addName))
		//					//		addName = $"{customerAddress.CompanyFirstName1} {customerAddress.CompanyLastName1} - {customerAddress.AddressID}]";

		//					//	addressList.Add(new ServiceAddress
		//					//	{
		//					//		Name = addName,
		//					//		Addr1 = customerAddress.Address,
		//					//		City = customerAddress.City,
		//					//		PostalCode = customerAddress.Zip,
		//					//		AddressId = customerAddress.AddressID.ToString()
		//					//	});
		//					//}

		//					//foreach (var customerAddress in addressList)
		//					//{
		//					//	var shipToAddress = requestSet.CreateElement("ShipToAddress");
		//					//	custMod.AppendChild(shipToAddress);

		//					//	shipToAddress.AppendChild(requestSet.CreateElement("Name")).InnerText = customerAddress.Name.ToCleanStringSub0(41);

		//					//	if (!string.IsNullOrEmpty(customerAddress.Addr1))
		//					//		shipToAddress.AppendChild(requestSet.CreateElement("Addr1")).InnerText = customerAddress.Addr1.ToCleanStringSub0(41);

		//					//	if (!string.IsNullOrEmpty(customerAddress.City))
		//					//		shipToAddress.AppendChild(requestSet.CreateElement("City")).InnerText = customerAddress.City.Trim().ToCleanStringSub0(31);

		//					//	if (!string.IsNullOrEmpty(customerAddress.PostalCode))
		//					//		shipToAddress.AppendChild(requestSet.CreateElement("PostalCode")).InnerText = customerAddress.PostalCode.Trim().ToCleanStringSub0(13);
		//					//	shipToAddress.AppendChild(requestSet.CreateElement("Country")).InnerText = "USA";
		//					//}


		//					#endregion

		//					custMod.AppendChild(requestSet.CreateElement("Notes")).InnerText = "SkyBoss ID " + cust.CustomerID;
		//					counter++;
		//				}
		//				else
		//				{
		//					var errorMessage =
		//						"Unable to find Customer information On QuickBook, QBDesktopID:" + cust.QBDesktopId?.Trim() + " Skyboss will Add Again in Quickbook";
		//					UpdateStatus(sess.GetUsername(),
		//						$"Customer ID: {cust.CustomerID.ToString()}, Message:{errorMessage} \n");
		//					var _customerService = new CustomerService();
		//					var getCustomer = _customerService.GetCustomerById(cust.CustomerID);
		//					var dbList = new List<int>();
		//					if (getCustomer != null)
		//					{
		//						getCustomer.IsQBSynced = false;
		//						getCustomer.QBDesktopId = null;
		//						_customerService.UpdateCustomer(getCustomer);
		//					}
		//				}
		//			}
		//			catch (Exception exp)
		//			{
		//				// exp.QBLogRequest("QuickBooksCustomerOps.BuildCustomerModRequest \t  => ");
		//				new QbLogService().Error("QuickBooksCustomerOps.BuildCustomerModRequest \t  => " + exp.Message + " => CustomerID: " + cust.CustomerID, exp);
		//				UpdateStatus(sess.GetUsername(),
		//					$"Customer ID: {cust.CustomerID.ToString()}, Message:{exp.Message} \n");

		//				//remove the current Request from Xml tree as exception occured and may cause error in QB
		//				var qbXMLMsgsRsNodeList = requestSet.GetElementsByTagName("QBXMLMsgsRq");
		//				var childNode =
		//					qbXMLMsgsRsNodeList.Cast<XmlNode>().Select(x => x.LastChild).FirstOrDefault();
		//				if (childNode != null) qbXMLMsgsRq.RemoveChild(childNode);
		//				listIds.RemoveAll(x => x.RequestId == counter.ToString());
		//			}

		//		sess.SetProperty(CustomersIds, listIds);
		//	}
		//	catch (Exception exp)
		//	{
		//		// exp.QBLogRequest("QuickBooksCustomerOps.GetCustomersFromQuery \t  => ");
		//		new QbLogService().Error("QuickBooksCustomerOps.GetCustomersFromQuery \t  => " + exp.Message, exp);
		//	}
		//}

		//#endregion

		//public void UpdateCustomer(string response, QuickBookSession sess)
		//{
		//	// response.QBLogRequest("QuickBooksCustomerOps.UpdateCustomer \t  => ");

		//	try
		//	{
		//		var outputXMLDoc = new XmlDocument();
		//		outputXMLDoc.LoadXml(response);
		//		var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerAddRs");
		//		qbXMLMsgsRsNodeList = qbXMLMsgsRsNodeList.Count > 0
		//			? qbXMLMsgsRsNodeList
		//			: outputXMLDoc.GetElementsByTagName("CustomerModRs");

		//		var customersIdsSess = sess.GetProperty(CustomersIds);
		//		var customersIdsList = (List<UtilityClass>)customersIdsSess;
		//		var _customerServices = new CustomerService();

		//		var count = qbXMLMsgsRsNodeList.Count;
		//		for (var x = 0; x < count; x++)
		//			try
		//			{
		//				var rsAttributes = qbXMLMsgsRsNodeList.Item(x).Attributes;
		//				var retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
		//				//var retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
		//				var retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;
		//				var retRequestID = rsAttributes.GetNamedItem("requestID").Value;

		//				//get the response node for detailed info  and a response contains max one childNode for "CustomerRet"
		//				var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(x).ChildNodes;

		//				var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);
		//				var custID = objIds != null ? Convert.ToInt32(objIds.EntityID) : 0;


		//				if (custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0).Name.Equals("CustomerRet") &&
		//					retStatusCode == "0" ||
		//					retStatusCode == "530"
		//				) //Record Updated 530 status mean record Exported But For Some Record Info Currently QB Not Providing Implementation Of it.
		//				{
		//					var i = GetCount(sess);
		//					if (i % ProgressSteps == 0 && i != 0)
		//						UpdateStatus(sess.GetUsername(),
		//							"Processed " + i +
		//							" Customers ..."); //show status of only those record which are processed successfully;

		//					var custRetNodeList = custAddRsNodeList.Item(0).ChildNodes;
		//					var qbDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID")
		//						.Select(z => z.InnerText).FirstOrDefault();
		//					if (objIds != null)
		//						objIds.QbDesktopId = qbDesktopId;
		//					// added new CustomerService again because random characters added in skyboss also.
		//					var _customerService = new CustomerService();

		//					var getCustomer = _customerService.GetCustomerById(custID);
		//					if (getCustomer != null && !string.IsNullOrEmpty(qbDesktopId))
		//					{
		//						getCustomer.IsQBSynced = true;
		//						getCustomer.QBDesktopId = qbDesktopId;
		//						_customerService.UpdateCustomer(getCustomer);
		//					}
		//				}
		//				else if (qbXMLMsgsRsNodeList.Item(x).Name == "CustomerAddRs" && retStatusCode == "3100"
		//																			 && IsDuplicateRecord1StTry(sess,
		//																				 custID)
		//				) //mean Duplicate record exists in Adding New Record In Qb make another try 
		//				{
		//					var getCustomer = _customerServices.GetCustomerById(custID);
		//					if (getCustomer != null)
		//					{
		//						var dbList = new List<int>();
		//						object lists = null;

		//						var dbListdup = new List<int>();
		//						object listsdup = null;

		//						if (sess.GetProperty(AddRequestDbList.CustomersLists.ToString()) != null)
		//							lists = sess.GetProperty(AddRequestDbList.CustomersLists.ToString());
		//						dbList = lists != null ? (List<int>)lists : dbList;

		//						if (sess.GetProperty(AddRequestDbList.CustomersDupLists.ToString()) != null)
		//							listsdup = sess.GetProperty(AddRequestDbList.CustomersDupLists.ToString());
		//						dbListdup = listsdup != null ? (List<int>)listsdup : dbListdup;

		//						getCustomer = AddRandomNameWihCust(getCustomer);
		//						dbListdup.Add(getCustomer.CustomerID);
		//						dbList.Add(getCustomer.CustomerID);
		//						dbListdup.ToList();
		//						dbList.ToList();


		//						sess.SetProperty(AddRequestDbList.CustomersDupLists.ToString(), dbListdup.ToList());
		//						sess.SetProperty(AddRequestDbList.CustomersLists.ToString(), dbList.ToList());
		//						PutDuplicateRecordInfo(sess, custID);
		//					}
		//				}
		//				else if (qbXMLMsgsRsNodeList.Item(x).Name == "CustomerModRs" &&
		//						 (retStatusCode == "3100" || retStatusCode == "3170") &&
		//						 IsDuplicateRecord1StTry(sess, custID)
		//				) //mean Duplicate record exists In modifying Record In Qb make another try 
		//				{
		//					var getCustomer = _customerServices.GetCustomerById(custID);
		//					if (getCustomer != null)
		//					{
		//						var dbList = new List<int>();
		//						object lists = null;

		//						var dbListdup = new List<int>();
		//						object listsdup = null;

		//						if (sess.GetProperty(ModRequestDbList.CustomersModLists.ToString()) != null)
		//							lists = sess.GetProperty(ModRequestDbList.CustomersModLists.ToString());
		//						dbList = lists != null ? (List<int>)lists : dbList;

		//						if (sess.GetProperty(ModRequestDbList.CustomersDupModLists.ToString()) != null)
		//							listsdup = sess.GetProperty(ModRequestDbList.CustomersDupModLists.ToString());
		//						dbListdup = listsdup != null ? (List<int>)listsdup : dbListdup;
		//						getCustomer = AddRandomNameWihCust(getCustomer);
		//						dbListdup.Add(getCustomer.CustomerID);
		//						dbList.Add(getCustomer.CustomerID);
		//						dbListdup.ToList();
		//						dbList.ToList();

		//						sess.SetProperty(ModRequestDbList.CustomersDupModLists.ToString(), dbListdup.ToList());
		//						sess.SetProperty(ModRequestDbList.CustomersModLists.ToString(), dbList.ToList());
		//						PutDuplicateRecordInfo(sess, custID);
		//					}
		//				}
		//				else //mean Data is not Export to QB
		//				{
		//					UpdateStatus(sess.GetUsername(),
		//						$"Customer ID: {custID.ToString()}, StatusCode:{retStatusCode}, Message:{retStatusMessage} \n");
		//				}
		//			}

		//			catch (Exception exp)
		//			{
		//				// exp.QBLogRequest("QuickBooksCustomerOps.UpdateCustomer \t  => ");
		//				new QbLogService().Error("QuickBooksCustomerOps.UpdateCustomer \t  => " + exp.Message, exp);

		//				var rsAttributes = qbXMLMsgsRsNodeList.Item(x).Attributes;
		//				var retRequestID = rsAttributes.GetNamedItem("requestID").Value;
		//				var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);
		//				var custID = objIds != null ? Convert.ToInt32(objIds.EntityID) : 0;

		//				UpdateStatus(sess.GetUsername(),
		//					$"Customer ID: {custID.ToString()}, Message:{exp.Message} \n");
		//			}

		//		////add Those ServiceAddress id to cache so later we can get it and update these address in Qb as Qb at a time add 99 ship to address for a customer
		//		//AddServicesToSess(sess, CustomersIdsList);
		//	}
		//	catch (Exception exp)
		//	{
		//		// exp.QBLogRequest("QuickBooksCustomerOps.UpdateCustomer \t  => ");
		//		new QbLogService().Error("QuickBooksCustomerOps.UpdateCustomer \t  => " + exp.Message, exp);
		//		UpdateStatus(sess.GetUsername(), "Error in Customers Export, Message:  " + exp.Message);
		//	}

		//	sess.RemoveProperty(CustomersIds);
		//}

		//public void GetCustomersFromQuery(string response, QuickBookSession sess)
		//{
		//	//   response.QBLogRequest("QuickBooksCustomerOps.GetCustomersFromQuery \t  => ");


		//	try
		//	{

		//		var listIds = GetQueryListIds(sess);
		//		var outputXMLDoc = new XmlDocument();
		//		outputXMLDoc.LoadXml(response);
		//		var qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerQueryRs");

		//		var customersIdsSess = sess.GetProperty(CustomersIds);
		//		var customersIdsList = (List<UtilityClass>)customersIdsSess;

		//		var count = qbXMLMsgsRsNodeList.Count;
		//		for (var x = 0; x < count; x++)
		//			try
		//			{
		//				var rsAttributes = qbXMLMsgsRsNodeList.Item(x)?.Attributes;
		//				var retStatusCode = rsAttributes?.GetNamedItem("statusCode").Value;
		//				//var retStatusSeverity = rsAttributes?.GetNamedItem("statusSeverity").Value;
		//				//var retStatusMessage = rsAttributes?.GetNamedItem("statusMessage").Value;
		//				var retRequestID = rsAttributes?.GetNamedItem("requestID").Value;

		//				//get the response node for detailed info  and a response contains max one childNode for "CustomerRet"
		//				var custAddRsNodeList = qbXMLMsgsRsNodeList.Item(x)?.ChildNodes;
		//				if (custAddRsNodeList != null &&
		//					(custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0)?.Name == "CustomerRet" &&
		//					 (retStatusCode == "0" || retStatusCode == "530")))
		//				{
		//					var custRetNodeList = custAddRsNodeList.Item(0)?.ChildNodes;
		//					var qbDesktopId = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "ListID")
		//						.Select(z => z.InnerText).FirstOrDefault();
		//					var editSequence = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "EditSequence")
		//						.Select(z => z.InnerText).FirstOrDefault();
		//					var name = custRetNodeList?.Cast<XmlNode>().Where(y => y.Name == "FullName")
		//						.Select(z => z.InnerText).FirstOrDefault();

		//					var objIds = customersIdsList.FirstOrDefault(y => y.RequestId == retRequestID);

		//					var idsClass = new UtilityClass
		//					{
		//						RequestId = x.ToString(),
		//						EntityID = objIds.EntityID,
		//						EditSequence = editSequence,
		//						QbDesktopId = qbDesktopId,
		//						Name = name
		//					};
		//					listIds.Add(idsClass);
		//				}
		//			}
		//			catch (Exception exp)
		//			{
		//				//exp.QBLogRequest("QuickBooksCustomerOps.GetCustomersFromQuery \t  => ");
		//				new QbLogService().Error("QuickBooksCustomerOps.GetCustomersFromQuery \t  => " + exp.Message, exp);
		//			}


		//		//remove previous Ids and add new List and Edit sequence against those entity which are going to update
		//		sess.RemoveProperty(CustomersIds);
		//		SetQueryListIds(sess, listIds);

		//	}
		//	catch (Exception exp)
		//	{
		//		// exp.QBLogRequest("QuickBooksCustomerOps.GetCustomersFromQuery \t  => ");
		//		new QbLogService().Error("QuickBooksCustomerOps.GetCustomersFromQuery \t  => " + exp.Message, exp);
		//	}


		//}

		//#endregion

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

								for (var i = 0; i < length; i++)
									try
									{
										if (custAddRsNodeList.Count > 1 && custAddRsNodeList.Item(i)?.Name == "CustomerRet" &&
											(retStatusCode == "0" || retStatusCode == "530"))
										{
											var c = GetCount(sess);

											var custRetNodeList = custAddRsNodeList.Item(i)?.ChildNodes;
											var customer = MapQBCustomerToPartner(custRetNodeList);

											await _partnerRepository.InsertAsync(customer);
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

		private Partner MapQBCustomerToPartner(XmlNodeList custRetNodeList)
		{
			var custRetNode = custRetNodeList.Cast<XmlNode>();


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


			var partner = new Partner
			{
				QuickBooksDesktopId = custRetNodeList.Cast<XmlNode>().Where(y => y.Name == "ListID").Select(z => z.InnerText)
					.FirstOrDefault(),
				FullName = !string.IsNullOrEmpty(companyName) ? companyName.Trim() : ""
			};

			partner.FullName = fullName;
			partner.Name = !string.IsNullOrEmpty(name) ? name : firstName + " " + lastName;
			partner.LastModificationTime = timeModified.FromTimeStampToDate();
			//partner.LastModifierUserId = IAbpSession.UserId;

			return partner;
		}

		#endregion


	}
}
