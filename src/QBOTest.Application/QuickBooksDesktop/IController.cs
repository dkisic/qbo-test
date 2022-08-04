using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	public interface IController
	{
		bool HaveAnyWork(QuickBookSession sess);
		bool HaveAccounts();
		bool HaveImportSetting(QuickBookSession sess);
		//string GetNextAction(QuickBookSession sess);
		//int ProcessLastAction(QuickBookSession sess, string response);
		string GetNextActionForImport(QuickBookSession sess);
		Task<int> ProcessLastActionForImportAsync(QuickBookSession sess, string response);

	}
}
