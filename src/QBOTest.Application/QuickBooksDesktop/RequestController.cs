using Abp.Domain.Repositories;
using QBOTest.Partners;
using QBOTest.QuickBooksDesktop.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QBOTest.QuickBooksDesktop
{


	public class RequestController
	: IController
	{
		/// <summary>
		///     In general we transition from Do*Query to Get*Response,
		///     but if there is more data than will fit in one response,
		///     then we go into a More*Query state which alternates
		/// </summary>
		public enum ControllerStates
		{
			//Start = 0,
			Preflight = 0,

			CheckQueryRq,
			CheckQueryRs,

			CustomerAddRq,
			CustomerAddRs,
			CustomerQueryRq,
			CustomerQueryRs,
			CustomerModRq,
			CustomerModRs,

			PostFlight,
			End = 9999
		}

		public enum AddRequestDbList
		{
			CustomersLists = 0

		}
		public enum QueryRequestDbList
		{
			CustomersQueryLists = 0
		}
		public enum ModRequestDbList
		{
			CustomersModLists = 0
		}

		public enum ImportRequestList
		{
			CustomerImport
		}

		private readonly BaseClass baseClass = new BaseClass();

		private bool dataToExchange;
		private bool doImportData;
		private bool isAccountPresents;

		private readonly IRepository<Partner, Guid> _partnerRepository;

		public RequestController(IRepository<Partner, Guid> partnerRepository)
		{
			_partnerRepository = partnerRepository;
		}

		/// <summary>
		///     Called by authenticate to determine if any data to exchange.
		/// </summary>
		/// <param name="sess"></param>
		/// <returns>true/false</returns>
		public bool HaveAnyWork(QuickBookSession sess)
		{
			//QBLogHelper.QBLogRequest($"RequestController.HaveAnyWork \t  ");


			//Count new Data in database 
			CountNewData();
			// Return status 
			return dataToExchange;
		}

		/// <summary>
		///     Called by authenticate to determine is user enter their Account Setting.
		/// </summary>
		/// <returns>true/false</returns>
		public bool HaveAccounts()
		{
			CheckForAccounts();
			// Return status 
			return isAccountPresents;
		}

		/// <summary>
		///     Called by authenticate & sendRequestXML & receiveResponseXML to determine is user want to import Data from
		///     QuickBook to Skyboss.
		/// </summary>
		/// <param name="sess">QuickBookSession</param>
		/// <returns>true/false</returns>
		public bool HaveImportSetting(QuickBookSession sess)
		{
			//CheckForImportSetting(sess);
			return doImportData;
		}



		/// <summary>
		///     Called by sendRequestXML to determine what to send next.
		///     Corresponds to states Start and *Query states.
		/// </summary>
		/// <param name="sess"></param>
		/// <returns>String</returns>
		//public string GetNextAction(QuickBookSession sess)
		//{

		//	if (sess == null) throw new Exception("getNextAction: invalid session object");
		//	var action = "";

		//	var controllerState = ControllerStates.CheckQueryRq;
		//	var subControllerState = SubControllerStates.CustomerCheckRq;
		//	if (sess?.GetProperty("controllerState") != null)
		//		controllerState = (ControllerStates)sess?.GetProperty("controllerState");
		//	if (sess.GetProperty("subControllerState") != null)
		//		subControllerState = (SubControllerStates)sess.GetProperty("subControllerState");

		//	var lastControllerState = ControllerStates.CheckQueryRs;
		//	var subLastControllerState = SubControllerStates.CustomerCheckRs;
		//	if (sess?.GetProperty("lastControllerState") != null)
		//		lastControllerState = (ControllerStates)sess?.GetProperty("lastControllerState");
		//	if (sess.GetProperty("subLastControllerState") != null)
		//		subLastControllerState = (SubControllerStates)sess?.GetProperty("subLastControllerState");

		//	switch (controllerState)
		//	{
		//		#region Preflight

		//		case ControllerStates.Preflight:
		//			// Ask user if they want to do this update or not
		//			// if OK,       controllerState=ControllerStates.CustomerAddRq
		//			// if Cancel,   controllerState=ControllerStates.End
		//			//dbm.SetInteractiveMode(sess.getTicket(), "NEEDED");
		//			action = ""; //The empty string will trigger getLastError() which will start interactive mode.
		//			lastControllerState = ControllerStates.Preflight;
		//			controllerState = ControllerStates.CheckQueryRq;
		//			break;

		//		#endregion

		//		#region Check

		//		case ControllerStates.CheckQueryRq: // Build and send Customer Add Request xml
		//			var checkOps = new QuickBooksEntityCheckOps();
		//			switch (subControllerState)
		//			{
		//				case SubControllerStates.CustomerCheckRq:
		//					action = checkOps.QueryCustomers(sess);
		//					subLastControllerState = SubControllerStates.CustomerCheckRq;
		//					subControllerState = SubControllerStates.CustomerCheckRs;
		//					break;

		//			}

		//			lastControllerState = ControllerStates.CheckQueryRq;
		//			controllerState = ControllerStates.CheckQueryRs;
		//			break;

		//		#endregion

		//		#region Customer

		//		case ControllerStates.CustomerAddRq: // Build and send Customer Add Request xml
		//			var qbCustomerOps = new QuickBooksCustomerOps(_partnerRepository);
		//			action = qbCustomerOps.AddCustomers(sess);

		//			lastControllerState = ControllerStates.CustomerAddRq;
		//			controllerState = ControllerStates.CustomerAddRs;
		//			break;
		//		case ControllerStates.CustomerQueryRq: // Build and send Customer Query Request xml
		//			var qbCustomerOps0 = new QuickBooksCustomerOps(_partnerRepository);
		//			action = qbCustomerOps0.QueryCustomers(sess);

		//			lastControllerState = ControllerStates.CustomerAddRq;
		//			controllerState = ControllerStates.CustomerQueryRs;
		//			break;
		//		case ControllerStates.CustomerModRq: // Build and send Customer Modification Request xml
		//			var qbCustomerOps1 = new QuickBooksCustomerOps(_partnerRepository);
		//			action = qbCustomerOps1.ModCustomers(sess);

		//			lastControllerState = ControllerStates.CustomerModRq;
		//			controllerState = ControllerStates.CustomerModRs;
		//			break;

		//			#endregion

		//	}

		//	sess.SetProperty("lastControllerState", lastControllerState);
		//	sess.SetProperty("controllerState", controllerState);
		//	sess.SetProperty("subLastControllerState", subLastControllerState);
		//	sess.SetProperty("subControllerState", subControllerState);
		//	return action;
		//}

		///// <summary>
		/////     Called by receiveResponseXML to process the last data received.
		/////     Corresponds to states Get*Response
		///// </summary>
		///// <param name="sess"></param>
		///// <param name="response"></param>
		///// <returns>int</returns>
		//public int ProcessLastAction(QuickBookSession sess, string response)
		//{

		//	var completion = 0;
		//	var oldLastControllerState = ControllerStates.CheckQueryRq;
		//	var subOldLastControllerState = SubControllerStates.CustomerCheckRq;

		//	var controllerState = ControllerStates.CheckQueryRs;
		//	var subControllerState = SubControllerStates.CustomerCheckRs;
		//	if (sess?.GetProperty("controllerState") != null)
		//		controllerState = (ControllerStates)sess.GetProperty("controllerState");
		//	if (sess?.GetProperty("controllerState") != null)
		//		subControllerState = (SubControllerStates)sess.GetProperty("subControllerState");


		//	var lastControllerState = ControllerStates.CheckQueryRq;
		//	var subLastControllerState = SubControllerStates.CustomerCheckRq;

		//	if (sess?.GetProperty("lastControllerState") != null)
		//	{
		//		lastControllerState = (ControllerStates)sess.GetProperty("lastControllerState");
		//		oldLastControllerState = lastControllerState;
		//	}

		//	if (sess?.GetProperty("subLastControllerState") != null)
		//	{
		//		subLastControllerState = (SubControllerStates)sess.GetProperty("subLastControllerState");
		//		subOldLastControllerState = subLastControllerState;
		//	}

		//	//new QbLogService().Information("QBDesktop:\n" + controllerState + "\n" + response); temporily commented the xml log creation by wajahat

		//	switch (controllerState)
		//	{
		//		#region Check

		//		case ControllerStates.CheckQueryRs: // Build and send Customer Add Request xml
		//			var checkOps = new QuickBooksEntityCheckOps();
		//			switch (subControllerState)
		//			{
		//				case SubControllerStates.CustomerCheckRs:
		//					checkOps.GetCustomersFromQuery(response, sess);
		//					subLastControllerState = SubControllerStates.CustomerCheckRs;
		//					subControllerState = SubControllerStates.End;
		//					break;

		//			}

		//			lastControllerState = ControllerStates.CheckQueryRq;
		//			controllerState = subControllerState == SubControllerStates.End ? ControllerStates.CustomerAddRq : ControllerStates.CheckQueryRq;

		//			completion = 4;
		//			break;

		//		#endregion

		//		#region Customers

		//		case ControllerStates.CustomerAddRs: //customer Add response
		//			var qbCustomerOps = new QuickBooksCustomerOps(_partnerRepository);
		//			qbCustomerOps.UpdateCustomer(response, sess);

		//			lastControllerState = ControllerStates.CustomerAddRs;
		//			controllerState = ControllerStates.CustomerQueryRq;
		//			completion = 5;
		//			break;
		//		case ControllerStates.CustomerQueryRs: //customer Query response
		//			var qbCustomerOps0 = new QuickBooksCustomerOps(_partnerRepository);
		//			qbCustomerOps0.GetCustomersFromQuery(response, sess);

		//			lastControllerState = ControllerStates.CustomerQueryRs;
		//			controllerState = ControllerStates.CustomerModRq;
		//			completion = 6;
		//			break;
		//		case ControllerStates.CustomerModRs: //customer Mod response
		//			var qbCustomerOps1 = new QuickBooksCustomerOps(_partnerRepository);
		//			qbCustomerOps1.UpdateCustomer(response, sess);

		//			lastControllerState = ControllerStates.CustomerModRs;
		//			controllerState = ControllerStates.End;
		//			completion = 10;
		//			break;

		//		#endregion

		//		case ControllerStates.End:
		//			//Finally, clean up work.
		//			completion = 100;
		//			break;


		//		#region Default

		//		default:
		//			sess?.SetProperty("LastError", "processLastAction: Unexpected state: " + controllerState);
		//			//throw new Exception("processLastAction: Unexpected state: " + controllerState);
		//			break;

		//			#endregion
		//	}

		//	if (CheckForLastRequestWorkMore(sess, lastControllerState) == false)
		//	{
		//		sess?.SetProperty("lastControllerState", lastControllerState);
		//		sess?.SetProperty("controllerState", controllerState);
		//	}
		//	else
		//	{
		//		//mean again process last Request again more data to sync
		//		sess?.SetProperty("lastControllerState", oldLastControllerState);
		//		sess?.SetProperty("controllerState", oldLastControllerState);
		//	}

		//	if (CheckFoWorkMoreForCheckEntity(sess, subLastControllerState) == false)
		//	{
		//		sess?.SetProperty("subLastControllerState", subLastControllerState);
		//		sess?.SetProperty("subControllerState", subControllerState);
		//	}
		//	else
		//	{
		//		//mean again process last Request again more data to sync
		//		sess?.SetProperty("subLastControllerState", subOldLastControllerState);
		//		sess?.SetProperty("subControllerState", subOldLastControllerState);
		//	}

		//	return completion;
		//}


		public string GetNextActionForImport(QuickBookSession sess)
		{

			var controllerState = ControllerStates.CustomerQueryRq;

			if (sess?.GetProperty("controllerState") != null)
				controllerState = (ControllerStates)sess.GetProperty("controllerState");

			var lastControllerState = ControllerStates.CustomerQueryRs;
			if (sess?.GetProperty("lastControllerState") != null)
				lastControllerState = (ControllerStates)sess.GetProperty("lastControllerState");

			string action;
			switch (controllerState)
			{
				case ControllerStates.CustomerQueryRq: // Build and send Customer Query Request xml
					var qbCustomerOps = new QuickBooksCustomerOps(_partnerRepository);
					action = qbCustomerOps.QueryCustomersForImport(sess);

					lastControllerState = ControllerStates.CustomerQueryRq;
					controllerState = ControllerStates.CustomerQueryRs;
					break;


				case ControllerStates.End:
					// Send a dummy Query so the control flows into processLastAction()
					action = baseClass.EmptyRequest();

					break;
				default:
					sess?.SetProperty("LastError", "getNextAction: Unexpected state: " + controllerState);
					action = "";
					break;
			}

			sess?.SetProperty("lastControllerState", lastControllerState);
			sess?.SetProperty("controllerState", controllerState);
			return action;
		}

		public async Task<int> ProcessLastActionForImportAsync(QuickBookSession sess, string response)
		{


			var completion = 0;
			var oldLastControllerState = ControllerStates.CustomerQueryRs;

			var controllerState = ControllerStates.CustomerAddRs;
			if (sess.GetProperty("controllerState") != null)
				controllerState = (ControllerStates)sess.GetProperty("controllerState");

			var lastControllerState = ControllerStates.CustomerQueryRs;
			if (sess.GetProperty("lastControllerState") != null)
			{
				lastControllerState = (ControllerStates)sess.GetProperty("lastControllerState");
				oldLastControllerState = lastControllerState;
			}


			switch (controllerState)
			{
				case ControllerStates.CustomerQueryRs: //customer Query response
					var qbCustomerOps = new QuickBooksCustomerOps(_partnerRepository);
					await qbCustomerOps.GetCustomersFromQueryImportAsync(response, sess);

					lastControllerState = ControllerStates.CustomerQueryRs;
					controllerState = ControllerStates.End;
					completion = 25;
					break;


				case ControllerStates.End:
					//Finally, clean up work.
					completion = 100;
					break;

				default:
					sess.SetProperty("LastError", "processLastAction: Unexpected state: " + controllerState);
					//throw new Exception("processLastAction: Unexpected state: " + controllerState);
					break;
			}

			if (CheckForLastRequestWorkMoreImport(sess, lastControllerState) == false)
			{
				sess.SetProperty("lastControllerState", lastControllerState);
				sess.SetProperty("controllerState", controllerState);
			}
			else
			{
				//mean again process last Request again more data to sync
				sess.SetProperty("lastControllerState", oldLastControllerState);
				sess.SetProperty("controllerState", oldLastControllerState);
			}

			return completion;
		}




		private enum SubControllerStates
		{
			CustomerCheckRq,
			CustomerCheckRs,
			End
		}

		#region PRIVATE METHODS

		/// <summary>
		///     Check for data if not synced when authenticating user
		///     if no data is present for syncing then we send QB n work for you
		/// </summary>
		private void CountNewData()
		{
			// Need Implementation
			// Need to get new Customers to export
		}

		/// <summary>
		///     Check for Accounts Name when authenticating User
		///     AS we directly asking user to enter Accounts Name in QBD setting page
		///     if account info is missing then we info QBD we connector there is no work for you to do
		///     as Accounts for payments,expense etc is required That's why
		/// </summary>
		private void CheckForAccounts()
		{
			// Need to get Accounts name in case Services, POs, Invoice, or payments needs to be synced
		}


		/// <summary>
		///     AS we are sending only 100 records in each request and keep remaining data in session we check it in season does it
		///     have more data
		///     if it have more data then we repeat the same request until the season data is null
		/// </summary>
		/// <param name="sess"></param>
		/// <param name="oldControllerState"></param>
		/// <returns></returns>
		private bool CheckForLastRequestWorkMore(QuickBookSession sess, ControllerStates oldControllerState)
		{


			#region Customers

			if (oldControllerState == ControllerStates.CustomerAddRs)
				if (sess.GetProperty(AddRequestDbList.CustomersLists.ToString()) != null)
				{
					var lists = sess.GetProperty(AddRequestDbList.CustomersLists.ToString());
					var dbList = lists != null ? (List<int>)lists : new List<int>();
					if (dbList.Count == 0)
					{
						sess.RemoveProperty(AddRequestDbList.CustomersLists.ToString());

						baseClass.ClearCount(sess);
						baseClass.ClearDuplicateRecordInfo(sess);
					}

					return dbList.Count > 0; //if true more data is need to sync
				}

			if (oldControllerState == ControllerStates.CustomerQueryRs)
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) != null)
				{
					var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
					var dbList = lists != null ? (List<int>)lists : new List<int>();
					if (dbList.Count == 0)
						sess.RemoveProperty(QueryRequestDbList.CustomersQueryLists.ToString());

					return dbList.Count > 0; //if true more data is need to sync
				}

			if (oldControllerState == ControllerStates.CustomerModRs)
				if (sess.GetProperty(ModRequestDbList.CustomersModLists.ToString()) != null)
				{
					var lists = sess.GetProperty(ModRequestDbList.CustomersModLists.ToString());
					var dbList = lists != null ? (List<int>)lists : new List<int>();
					if (dbList.Count == 0)
					{
						sess.RemoveProperty(ModRequestDbList.CustomersModLists.ToString());

						baseClass.ClearCount(sess);
						baseClass.ClearDuplicateRecordInfo(sess);
						baseClass.RemoveQueryListIds(sess);
					}

					return dbList.Count > 0; //if true more data is need to sync
				}

			#endregion




			return false;
		}

		private bool CheckFoWorkMoreForCheckEntity(QuickBookSession sess, SubControllerStates subControllerStates)
		{
			//QBLogHelper.QBLogRequest("RequestController.CheckFoWorkMoreForCheckEntity \t  => ");

			#region Check

			if (subControllerStates == SubControllerStates.CustomerCheckRs)
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) != null)
				{
					var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
					var dbList = lists != null ? (List<int>)lists : new List<int>();
					if (dbList.Count == 0)
						sess.RemoveProperty(QueryRequestDbList.CustomersQueryLists.ToString());

					return dbList.Count > 0; //if true more data is need to sync
				}

			#endregion

			return false;
		}


		/// <summary>
		///     IteratorRemainingCount is send in Each response Of Iterator Query Request, We check if IteratorRemainingCount is
		///     not zero
		///     then send again request to Query remaining Data
		/// </summary>
		/// <param name="sess"></param>
		/// <param name="oldControllerState"></param>
		/// <returns></returns>
		private bool CheckForLastRequestWorkMoreImport(QuickBookSession sess, ControllerStates oldControllerState)
		{
			if (oldControllerState == ControllerStates.CustomerQueryRs)
				if (sess.GetProperty(ImportRequestList.CustomerImport.ToString()) != null)
				{
					var lists = sess.GetProperty(ImportRequestList.CustomerImport.ToString());
					var dbList = lists != null ? (UtilityClass)lists : new UtilityClass();
					if (dbList.IteratorRemainingCount.ToInt0() == 0)
					{
						sess.RemoveProperty(ImportRequestList.CustomerImport.ToString());
						baseClass.ClearCount(sess);
					}

					return dbList.IteratorRemainingCount.ToInt0() > 0; //if true more data is need to sync
				}



			return false;
		}










		/// <summary>
		///     Check For Sync Invoices FromDate Setting if User Entered in Qb setting
		///     if FromDate then syn only those invoices and their data from SB with QB
		/// </summary>
		/// <param name="sess"></param>


		private void GetUnsyncedPartners(QuickBookSession sess)
		{

			try
			{
				var UnsyncedPartners = _partnerRepository.GetAllList(x => x.QuickBooksDesktopId == null);
				sess.SetProperty(AddRequestDbList.CustomersLists.ToString(), UnsyncedPartners.DistinctBy(x => x.Id).Select(y=> y.Id).ToList());

			}
			catch (Exception exp)
			{
			}
		}



		#endregion

	}
}
