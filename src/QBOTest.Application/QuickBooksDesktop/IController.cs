using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	public interface IController
	{
		//bool HaveAnyWork(QuickBookSession sess);
		//bool HaveAccounts();
		//bool HaveImportSetting(QuickBookSession sess);
		//Task<string> GetNextAction(QuickBookSession sess);
		//Task<int> ProcessLastAction(QuickBookSession sess, string response);
		Task<string> GetNextActionForImport(QuickBookSession sess);
		Task<int> ProcessLastActionForImportAsync(QuickBookSession sess, string response);

	}
}
