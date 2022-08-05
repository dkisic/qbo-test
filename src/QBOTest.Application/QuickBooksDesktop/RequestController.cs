using Abp;
using Abp.Dependency;
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
	: IController, ITransientDependency
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
			CustomerModQueryRq,
			CustomerModQueryRs,
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
		private readonly QuickBooksCustomerOps _quickBooksCustomerOps;
		private readonly QuickBooksEntityCheckOps _quickBooksEntityCheckOps;


		public RequestController(IRepository<Partner, Guid> partnerRepository, QuickBooksCustomerOps quickBooksCustomerOps, QuickBooksEntityCheckOps quickBooksEntityCheckOps)
		{
			_partnerRepository = partnerRepository;
			_quickBooksCustomerOps = quickBooksCustomerOps;
			_quickBooksEntityCheckOps = quickBooksEntityCheckOps;

		}

		/// <summary>
		///     Called by sendRequestXML to determine what to send next.
		///     Corresponds to states Start and *Query states.
		/// </summary>
		/// <param name="sess"></param>
		/// <returns>String</returns>
		//public async Task<string> GetNextAction(QuickBookSession sess)
		//{

		//	if (sess == null) throw new Exception("getNextAction: invalid session object");
		//	var action = "";

		//	var controllerState = ControllerStates.CustomerQueryRq;
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

		//			action = ""; //The empty string will trigger getLastError() which will start interactive mode.
		//			lastControllerState = ControllerStates.Preflight;
		//			controllerState = ControllerStates.CheckQueryRq;
		//			break;

		//		#endregion

		//		//#region Check

		//		//case ControllerStates.CheckQueryRq: // Build and send Customer Add Request xml

		//		//	switch (subControllerState)
		//		//	{
		//		//		case SubControllerStates.CustomerCheckRq:
		//		//			action = (string) await _quickBooksCustomerOps.QueryCustomers(sess);
		//		//			subLastControllerState = SubControllerStates.CustomerCheckRq;
		//		//			subControllerState = SubControllerStates.CustomerCheckRs;
		//		//			break;

		//		//	}

		//		//	lastControllerState = ControllerStates.CheckQueryRq;
		//		//	controllerState = ControllerStates.CheckQueryRs;
		//		//	break;

		//		//#endregion

		//		#region Customer

		//		//case ControllerStates.CustomerAddRq: // Build and send Customer Add Request xml
		//		//	var qbCustomerOps = new QuickBooksCustomerOps(_partnerRepository);
		//		//	action = qbCustomerOps.AddCustomers(sess);

		//		//	lastControllerState = ControllerStates.CustomerAddRq;
		//		//	controllerState = ControllerStates.CustomerAddRs;
		//		//	break;
		//		case ControllerStates.CustomerQueryRq: // Build and send Customer Query Request xml

		//			action = (string)await _quickBooksCustomerOps.QueryCustomers(sess);

		//			lastControllerState = ControllerStates.CustomerQueryRq;
		//			controllerState = ControllerStates.CustomerQueryRs;
		//			break;
		//		case ControllerStates.CustomerModRq: // Build and send Customer Modification Request xml

		//			action = (string)await _quickBooksCustomerOps.ModCustomersAsync(sess);

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
		//public async Task<int> ProcessLastAction(QuickBookSession sess, string response)
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

		//			switch (subControllerState)
		//			{
		//				case SubControllerStates.CustomerCheckRs:
		//					await _quickBooksEntityCheckOps.GetCustomersFromQuery(response, sess);
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


		//		case ControllerStates.CustomerQueryRs: //customer Query response

		//			_quickBooksCustomerOps.GetCustomersFromQuery(response, sess);

		//			lastControllerState = ControllerStates.CustomerQueryRs;
		//			controllerState = ControllerStates.CustomerModRq;
		//			completion = 30;
		//			break;
		//		case ControllerStates.CustomerModRs: //customer Mod response

		//			_quickBooksCustomerOps.UpdateCustomer(response, sess);

		//			lastControllerState = ControllerStates.CustomerModRs;
		//			controllerState = ControllerStates.End;
		//			completion = 60;
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


		public async Task<string> GetNextActionForImport(QuickBookSession sess)
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
					action = _quickBooksCustomerOps.QueryCustomersForImport(sess);

					lastControllerState = ControllerStates.CustomerQueryRq;
					controllerState = ControllerStates.CustomerQueryRs;
					break;


				case ControllerStates.CustomerModQueryRq: // Build and send Customer Query Request xml

					action = (string)await _quickBooksCustomerOps.QueryCustomers(sess);

					lastControllerState = ControllerStates.CustomerModQueryRq;
					controllerState = ControllerStates.CustomerModQueryRs;
					break;
				case ControllerStates.CustomerModRq: // Build and send Customer Modification Request xml

					action = (string)await _quickBooksCustomerOps.ModCustomersAsync(sess);

					lastControllerState = ControllerStates.CustomerModRq;
					controllerState = ControllerStates.CustomerModRs;
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

			var controllerState = ControllerStates.CustomerModQueryRq;
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

					await _quickBooksCustomerOps.GetCustomersFromQueryImportAsync(response, sess);

					lastControllerState = ControllerStates.CustomerQueryRs;
					controllerState = ControllerStates.CustomerModQueryRq;
					completion = 30;
					break;

				case ControllerStates.CustomerModQueryRs: //customer Query response

					_quickBooksCustomerOps.GetCustomersFromQuery(response, sess);

					lastControllerState = ControllerStates.CustomerModQueryRs;
					controllerState = ControllerStates.CustomerModRq;
					completion = 50;
					break;
				case ControllerStates.CustomerModRs: //customer Mod response

					_quickBooksCustomerOps.UpdateCustomer(response, sess);

					lastControllerState = ControllerStates.CustomerModRs;
					controllerState = ControllerStates.End;
					completion = 60;
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
				}else{

					sess.SetProperty("controllerState", ControllerStates.CustomerModQueryRq);
				}

			if (oldControllerState == ControllerStates.CustomerModQueryRs)
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) != null)
				{
					var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
					var dbList = lists != null ? (List<Guid>)lists : new List<Guid>();
					if (dbList.Count == 0)
						sess.RemoveProperty(QueryRequestDbList.CustomersQueryLists.ToString());

					return dbList.Count > 0; //if true more data is need to sync
				}

			if (oldControllerState == ControllerStates.CustomerModRs)
				if (sess.GetProperty(ModRequestDbList.CustomersModLists.ToString()) != null)
				{
					var lists = sess.GetProperty(ModRequestDbList.CustomersModLists.ToString());
					var dbList = lists != null ? (List<Guid>)lists : new List<Guid>();
					if (dbList.Count == 0)
					{
						sess.RemoveProperty(ModRequestDbList.CustomersModLists.ToString());
						baseClass.ClearCount(sess);
						baseClass.RemoveQueryListIds(sess);
					}

					return dbList.Count > 0; //if true more data is need to sync
				}


			return false;
		}


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
						baseClass.RemoveQueryListIds(sess);
					}

					return dbList.Count > 0; //if true more data is need to sync
				}

			#endregion


			return false;
		}


		private bool CheckFoWorkMoreForCheckEntity(QuickBookSession sess, SubControllerStates subControllerStates)
		{


			if (subControllerStates == SubControllerStates.CustomerCheckRs)
				if (sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString()) != null)
				{
					var lists = sess.GetProperty(QueryRequestDbList.CustomersQueryLists.ToString());
					var dbList = lists != null ? (List<int>)lists : new List<int>();
					if (dbList.Count == 0)
						sess.RemoveProperty(QueryRequestDbList.CustomersQueryLists.ToString());

					return dbList.Count > 0; //if true more data is need to sync
				}


			return false;
		}


		#endregion

	}
}
